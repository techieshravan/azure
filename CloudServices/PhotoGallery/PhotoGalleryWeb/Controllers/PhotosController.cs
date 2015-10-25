using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoGallery.DB;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace PhotoGalleryWeb.Controllers
{
    public class PhotosController : Controller
    {
        private PhotoGalleryContext db = new PhotoGalleryContext();
        private CloudQueue imagesQueue;
        private static CloudBlobContainer imagesBlobContainer;

        public PhotosController()
        {
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString"));
            
            var blobClient = storageAccount.CreateCloudBlobClient();
            blobClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);
            imagesBlobContainer = blobClient.GetContainerReference("images");
            
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            imagesQueue = queueClient.GetQueueReference("images");
        }
        
        public async Task<ActionResult> Index()
        {  
            var photosList = db.Photos.AsQueryable();
            return View(await photosList.ToListAsync());
        }
        
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }
        
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Title, Description, Phone")] Photo photo, HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null; 
            
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    imageBlob = await UploadAndSaveBlobAsync(imageFile);
                    photo.ImageURL = imageBlob.Uri.ToString();
                }
                photo.PostedDate = DateTime.Now;
                db.Photos.Add(photo);
                await db.SaveChangesAsync();
                Trace.TraceInformation("Created PhotoId {0} in database", photo.PhotoId);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(photo.PhotoId.ToString());
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for PhotoId {0}", photo.PhotoId);
                }
                return RedirectToAction("Index");
            }

            return View(photo);
        }
        
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PhotoId, Title, Description, ImageURL, ThumbnailURL, PostedDate")] Photo photo, HttpPostedFileBase imageFile)
        {
            CloudBlockBlob imageBlob = null;
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength != 0)
                {
                    await DeleteAdBlobsAsync(photo);
                    imageBlob = await UploadAndSaveBlobAsync(imageFile);
                    photo.ImageURL = imageBlob.Uri.ToString();
                }

                db.Entry(photo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                Trace.TraceInformation("Updated PhotoId {0} in database", photo.PhotoId);

                if (imageBlob != null)
                {
                    var queueMessage = new CloudQueueMessage(photo.PhotoId.ToString());
                    await imagesQueue.AddMessageAsync(queueMessage);
                    Trace.TraceInformation("Created queue message for PhotoId {0}", photo.PhotoId);
                }
                return RedirectToAction("Index");
            }
            return View(photo);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = await db.Photos.FindAsync(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Photo photo = await db.Photos.FindAsync(id);

            await DeleteAdBlobsAsync(photo);

            db.Photos.Remove(photo);
            await db.SaveChangesAsync();
            Trace.TraceInformation("Deleted photo {0}", photo.PhotoId);
            return RedirectToAction("Index");
        }

        private async Task<CloudBlockBlob> UploadAndSaveBlobAsync(HttpPostedFileBase imageFile)
        {
            Trace.TraceInformation("Uploading image file {0}", imageFile.FileName);

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            
            CloudBlockBlob imageBlob = imagesBlobContainer.GetBlockBlobReference(blobName);
            
            using (var fileStream = imageFile.InputStream)
            {
                await imageBlob.UploadFromStreamAsync(fileStream);
            }

            Trace.TraceInformation("Uploaded image file to {0}", imageBlob.Uri.ToString());

            return imageBlob;
        }

        private async Task DeleteAdBlobsAsync(Photo photo)
        {
            if (!string.IsNullOrWhiteSpace(photo.ImageURL))
            {
                Uri blobUri = new Uri(photo.ImageURL);
                await DeleteAdBlobAsync(blobUri);
            }
            if (!string.IsNullOrWhiteSpace(photo.ThumbnailURL))
            {
                Uri blobUri = new Uri(photo.ThumbnailURL);
                await DeleteAdBlobAsync(blobUri);
            }
        }

        private static async Task DeleteAdBlobAsync(Uri blobUri)
        {
            string blobName = blobUri.Segments[blobUri.Segments.Length - 1];
            Trace.TraceInformation("Deleting image blob {0}", blobName);
            CloudBlockBlob blobToDelete = imagesBlobContainer.GetBlockBlobReference(blobName);
            await blobToDelete.DeleteAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}