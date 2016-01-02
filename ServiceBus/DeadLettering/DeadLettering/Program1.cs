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
    class Program1
    {
        static void Main(string[] args)
        {
            const string queueName = "MyDemoQueue1";

            var connectionString = CloudConfigurationManager.GetSetting("ServiceBusConnectionString");

            var nameSpaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            var queueDescription = new QueueDescription(queueName)
            {
                EnableDeadLetteringOnMessageExpiration = true,
                DefaultMessageTimeToLive = new TimeSpan(0, 1, 0)
            };

            if (!nameSpaceManager.QueueExists(queueName))
            {
                nameSpaceManager.CreateQueue(queueDescription);
            }

            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            for (int i = 1; i <= 100; i++)
            {
                var message = new BrokeredMessage("Hello From My Demo Queue" + i);

                //message.TimeToLive = new TimeSpan(0, 2, 0);

                message.Properties["TestProperty"] = "TestValue";

                Console.WriteLine("Sending Message: " + i);

                client.Send(message);

                System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine("Sending Messages Complete");

            Console.ReadLine();
        }
    }
}
