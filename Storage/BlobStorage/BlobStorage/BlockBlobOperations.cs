using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStorage
{
    class BlockBlobOperations
    {
        const string image = "Profile.png";

        public async Task UploadImageFileToBlobAsync()
        {
            CloudBlobContainer container = GetCloudBlobContainer();

            Console.WriteLine("Uploading to container");

            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                throw;
            }

            //container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            CloudBlockBlob blockBlob1 = container.GetBlockBlobReference(image);

            await blockBlob1.UploadFromFileAsync(image, FileMode.Open);
            
            CloudBlockBlob blockBlob2 = container.GetBlockBlobReference("mytextfile.txt");

            using (var fileStream = File.OpenRead(@"myfile.txt"))
            {
                await blockBlob2.UploadFromStreamAsync(fileStream);
            }

            Console.WriteLine("Upload Complete");
        }
        
        public void ListBlobs()
        {
            var cloudBlobContainer = GetCloudBlobContainer();
            
            Console.WriteLine("List of blobs in container");

            //foreach (IListBlobItem item in cloudBlobContainer.ListBlobs(null,true))
            foreach (IListBlobItem item in cloudBlobContainer.ListBlobs())
            {
                var blobType = item.GetType();

                if (blobType == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                }
                else if (blobType == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                }
                else if (blobType == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }
        }

        public async Task DownloadBlobAsync()
        {
            CloudBlobContainer container = GetCloudBlobContainer();
            
            Console.WriteLine("Downloading blob");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(image);

            await blockBlob.DownloadToFileAsync(string.Format("./CopyOf{0}", image), FileMode.Create);
        }

        public async Task ListBlobsSegmentedInFlatListingAsync()
        {
            var cloudBlobContainer = GetCloudBlobContainer();

            Console.WriteLine("List blobs in pages:");

            int i = 0;

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            do
            {
                resultSegment = await cloudBlobContainer.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 10, continuationToken, null, null);

                if (resultSegment.Results.Count() > 0)
                {
                    Console.WriteLine("Page {0}:", ++i);
                }

                foreach (var blobItem in resultSegment.Results)
                {
                    Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
                }

                Console.WriteLine();

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);
        }

        public async Task DeleteBlobAsync()
        {
            CloudBlobContainer container = GetCloudBlobContainer();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(image);

            Console.WriteLine("Delete block blob");

            await blockBlob.DeleteAsync();
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("democontainerblockblob");
            
            return container;
        }

        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
        
    }
}
