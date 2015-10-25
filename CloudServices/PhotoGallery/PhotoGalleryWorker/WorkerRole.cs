using System;
using System.Diagnostics;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using PhotoGallery.DB;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace PhotoGalleryWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private CloudQueue imagesQueue;
        private CloudBlobContainer imagesBlobContainer;
        private PhotoGalleryContext db;

        public override void Run()
        {
            Trace.TraceInformation("PhotoGalleryWorker entry point called");
            CloudQueueMessage msg = null;

            while (true)
            {
                try
                {

                    msg = this.imagesQueue.GetMessage();
                    if (msg != null)
                    {
                        ProcessQueueMessage(msg);
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch (StorageException e)
                {
                    if (msg != null && msg.DequeueCount > 5)
                    {
                        this.imagesQueue.DeleteMessage(msg);
                        Trace.TraceError("Deleting poison queue item: '{0}'", msg.AsString);
                    }
                    Trace.TraceError("Exception in PhotoGalleryWorker: '{0}'", e.Message);
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }

        private void ProcessQueueMessage(CloudQueueMessage msg)
        {
            Trace.TraceInformation("Processing queue message {0}", msg);

            var photoId = int.Parse(msg.AsString);
            Photo photo = db.Photos.Find(photoId);
            if (photo == null)
            {
                throw new Exception(String.Format("PhotoId {0} not found, can't create thumbnail", photoId.ToString()));
            }

            Uri blobUri = new Uri(photo.ImageURL);
            string blobName = blobUri.Segments[blobUri.Segments.Length - 1];

            CloudBlockBlob inputBlob = this.imagesBlobContainer.GetBlockBlobReference(blobName);
            string thumbnailName = Path.GetFileNameWithoutExtension(inputBlob.Name) + "thumb.jpg";
            CloudBlockBlob outputBlob = imagesBlobContainer.GetBlockBlobReference(thumbnailName);

            using (Stream input = inputBlob.OpenRead())
            using (Stream output = outputBlob.OpenWrite())
            {
                ConvertImageToThumbnailJPG(input, output);
                outputBlob.Properties.ContentType = "image/jpeg";
            }
            Trace.TraceInformation("Generated thumbnail in blob {0}", thumbnailName);

            photo.ThumbnailURL = outputBlob.Uri.ToString();
            db.SaveChanges();
            Trace.TraceInformation("Updated thumbnail URL in database: {0}", photo.ThumbnailURL);

            this.imagesQueue.DeleteMessage(msg);
        }

        public void ConvertImageToThumbnailJPG(Stream input, Stream output)
        {
            int thumbnailsize = 80;
            int width;
            int height;
            var originalImage = new Bitmap(input);

            if (originalImage.Width > originalImage.Height)
            {
                width = thumbnailsize;
                height = thumbnailsize * originalImage.Height / originalImage.Width;
            }
            else
            {
                height = thumbnailsize;
                width = thumbnailsize * originalImage.Width / originalImage.Height;
            }

            Bitmap thumbnailImage = null;
            try
            {
                thumbnailImage = new Bitmap(width, height);

                using (Graphics graphics = Graphics.FromImage(thumbnailImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(originalImage, 0, 0, width, height);
                }

                thumbnailImage.Save(output, ImageFormat.Jpeg);
            }
            finally
            {
                if (thumbnailImage != null)
                {
                    thumbnailImage.Dispose();
                }
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var dbConnString = CloudConfigurationManager.GetSetting("PhotoGalleryDbConnectionString");
            db = new PhotoGalleryContext(dbConnString);


            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));

            Trace.TraceInformation("Creating images blob container");
            var blobClient = storageAccount.CreateCloudBlobClient();
            imagesBlobContainer = blobClient.GetContainerReference("images");
            if (imagesBlobContainer.CreateIfNotExists())
            {

                imagesBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            Trace.TraceInformation("Creating images queue");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            imagesQueue = queueClient.GetQueueReference("images");
            imagesQueue.CreateIfNotExists();

            Trace.TraceInformation("Storage initialized");
            return base.OnStart();
        }
    }
    }
