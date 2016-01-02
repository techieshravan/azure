using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace DeadLettering
{
    class Program2
    {
        static void Main(string[] args)
        {
            const string queueName = "MyDemoQueue1";

            var connectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");
            
           // QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            string deadLetterQueueName = QueueClient.FormatDeadLetterPath(queueName);
            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, deadLetterQueueName);

            OnMessageOptions options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = TimeSpan.FromMinutes(1)
            };

            client.OnMessage((message) =>
            {
                try
                {
                    Console.WriteLine("Body: {0}, MessageID: {1}, Test Property: {2}", message.GetBody<string>(), message.MessageId, message.Properties["TestProperty"]);
                    message.Complete();
                }
                catch (Exception)
                {
                    message.Abandon();
                }
            }, options);

            Console.WriteLine("Receving Messages Complete, Press Any Key to Exit!");
            Console.ReadLine();
        }
    }
}
