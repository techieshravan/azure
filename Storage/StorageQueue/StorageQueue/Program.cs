using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Threading.Tasks;

namespace StorageQueueSample
{
    public class Program
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Azure Storage Queue Sample\n");

            CloudQueue queue = CreateQueueAsync().Result;

            BasicQueueOperationsAsync(queue).Wait();

            //UpdateEnqueuedMessageAsync(queue).Wait();

            //ProcessBatchOfMessagesAsync(queue).Wait();
            
            Console.WriteLine("Press any key to exit");

            Console.Read();
        }

        private static async Task<CloudQueue> CreateQueueAsync()
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            Console.WriteLine("1. Create a queue for the demo");
            CloudQueue queue = queueClient.GetQueueReference("samplequeue1");
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return queue;
        }

        private static async Task BasicQueueOperationsAsync(CloudQueue queue)
        {
            Console.WriteLine("2. Insert a single message into a queue");

            await queue.AddMessageAsync(new CloudQueueMessage("Hello World!"));

            Console.WriteLine("3. Peek at the next message");
            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync();

            if (peekedMessage != null)
            {
                Console.WriteLine("The peeked message is: {0}", peekedMessage.AsString);
            }

            Console.WriteLine("4. De-queue the next message");
            CloudQueueMessage message = await queue.GetMessageAsync();
            if (message != null)
            {
                Console.WriteLine("Processing & deleting message with content: {0}", message.AsString);
                await queue.DeleteMessageAsync(message);
            }
        }

        private static async Task UpdateEnqueuedMessageAsync(CloudQueue queue)
        {
            Console.WriteLine("5. Insert another test message ");
            await queue.AddMessageAsync(new CloudQueueMessage("Hello World Again!"));

            Console.WriteLine("6. Change the contents of a queued message");
            CloudQueueMessage message = await queue.GetMessageAsync();

            message.SetMessageContent("Updated contents.");
            await queue.UpdateMessageAsync(message, TimeSpan.Zero, MessageUpdateFields.Content | MessageUpdateFields.Visibility);
        }

        private static async Task ProcessBatchOfMessagesAsync(CloudQueue queue)
        {
            Console.WriteLine("7. Enqueue 20 messages.");
            for (int i = 0; i < 20; i++)
            {
                await queue.AddMessageAsync(new CloudQueueMessage(string.Format("{0} - {1}", i, "Hello World")));
            }

            Console.WriteLine("8. Get the queue length");
            queue.FetchAttributes();
            int? cachedMessageCount = queue.ApproximateMessageCount;

            Console.WriteLine("Number of messages in queue: {0}", cachedMessageCount);

            Console.WriteLine("9. Dequeue 21 messages, allowing 5 minutes for the clients to process.");
            foreach (CloudQueueMessage msg in await queue.GetMessagesAsync(21, TimeSpan.FromMinutes(5), null, null))
            {
                Console.WriteLine("Processing & deleting message with content: {0}", msg.AsString);

                await queue.DeleteMessageAsync(msg);
            }
        }


        private static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        private static async Task DeleteQueueAsync(CloudQueue queue)
        {
            Console.WriteLine("10. Delete the queue");
            await queue.DeleteAsync();
        }
    }
}
