using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;

namespace Queue1
{
    class Sender
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            var nameSpaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!nameSpaceManager.QueueExists("MyQueue1"))
            {
                nameSpaceManager.CreateQueue("MyQueue1");
            }

            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, "MyQueue1");

            for (int i = 0; i < 100; i++)
            {
                var message = new BrokeredMessage("Hello From my Queue" + i);

                message.Properties["TestProperty"] = "TestValue";

                client.Send(message);
                System.Threading.Thread.Sleep(2000);

            }
            

            Console.WriteLine("Sending Messages Complete");
            Console.ReadLine();
        }
    }
}
