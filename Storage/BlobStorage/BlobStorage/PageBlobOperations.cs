using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStorage
{
    class PageBlobOperations
    {
        public async Task BasicStoragePageBlobOperations()
        {
            const string PageBlobName = "samplepageblob";

            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            Console.WriteLine("1. Creating Container");
            CloudBlobContainer container = blobClient.GetContainerReference("democontainerpageblob");
            await container.CreateIfNotExistsAsync();

            Console.WriteLine("2. Creating Page Blob");
            CloudPageBlob pageBlob = container.GetPageBlobReference(PageBlobName);
            
            Console.WriteLine("2. Write to a Page Blob");
            byte[] samplePagedata = new byte[512];
            Random random = new Random();
            random.NextBytes(samplePagedata);
            await pageBlob.UploadFromByteArrayAsync(samplePagedata, 0, samplePagedata.Length);

            Console.WriteLine("3. List Blobs in Container");
            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;
                foreach (IListBlobItem blob in resultSegment.Results)
                {
                    Console.WriteLine("{0} (type: {1}", blob.Uri, blob.GetType());
                }
            } while (token != null);

            int bytesRead = await pageBlob.DownloadRangeToByteArrayAsync(samplePagedata, 0, 0, samplePagedata.Count());

            Console.WriteLine("5. Delete page Blob");
            await pageBlob.DeleteAsync();
        }

        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
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
