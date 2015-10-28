using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;

namespace Queue1
{
    class Receiver
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, "MyQueue1");

            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = false;
            options.AutoRenewTimeout = TimeSpan.FromMinutes(1);

            client.OnMessage((message) =>
            {
                try
                {
                    Console.WriteLine("Body: " + message.GetBody<string>());
                    Console.WriteLine("MessageID: " + message.MessageId);
                    Console.WriteLine("Test Property: " + message.Properties["TestProperty"]);
                    message.Complete();
                }
                catch (Exception)
                {
                    message.Abandon();
                }
            }, options);

            Console.WriteLine("Receving Messages Complete");
            Console.ReadLine();
        }
    }
}
