using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace BlobStorage
{
    class AppendBlobOperations
    {
        public void UploadToAppendBlob()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            CloudBlobContainer container = blobClient.GetContainerReference("my-append-blobs");
            
            container.CreateIfNotExists();
            
            CloudAppendBlob appendBlob = container.GetAppendBlobReference("append-blob.log");
            
            if (!appendBlob.Exists())
            {
                appendBlob.CreateOrReplace();
            }
            
            int numBlocks = 10;

            Random rnd = new Random();
            byte[] bytes = new byte[numBlocks];
            rnd.NextBytes(bytes);

            //Simulate a logging operation by writing text data and byte data to the end of the append blob.
            for (int i = 0; i < numBlocks; i++)
            {
                appendBlob.AppendText(string.Format("Timestamp: {0:u} \tLog Entry: {1}{2}", DateTime.UtcNow, bytes[i], Environment.NewLine));
            }

            Console.WriteLine(appendBlob.DownloadText());
        }
    }
}
