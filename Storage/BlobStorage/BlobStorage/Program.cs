using System;

namespace BlobStorage
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure  Blob Storage\n ");

            var blockBlobOperations = new BlockBlobOperations();
            blockBlobOperations.UploadImageFileToBlob();
            blockBlobOperations.ListBlobs();
            blockBlobOperations.DownloadBlob();

            var appendBlobOperations = new AppendBlobOperations();
            appendBlobOperations.UploadToAppendBlob();

            var pageBlobOperations = new PageBlobOperations();
            pageBlobOperations.BasicStoragePageBlobOperations();

            Console.ReadLine();
        }
    }
}
