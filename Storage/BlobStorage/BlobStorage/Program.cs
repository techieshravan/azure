using System;

namespace BlobStorage
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure  Blob Storage\n ");

            var blockBlobOperations = new BlockBlobOperations();
            blockBlobOperations.UploadImageFileToBlobAsync().Wait();
            blockBlobOperations.ListBlobs();
            blockBlobOperations.DownloadBlobAsync().Wait();

            var appendBlobOperations = new AppendBlobOperations();
            appendBlobOperations.UploadToAppendBlob();

            var pageBlobOperations = new PageBlobOperations();
            pageBlobOperations.BasicPageBlobOperationsAsync().Wait();

            Console.ReadLine();
        }
    }
}
