using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;

namespace QueueReceiver2
{
    class Receiver
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            QueueClient queueClient = QueueClient.CreateFromConnectionString(connectionString, "MySampleQueue2");

            Console.WriteLine("\nReceiving message from Queue...");

            BrokeredMessage message = null;
            while (true)
            {
                try
                {  
                    message = queueClient.Receive(TimeSpan.FromSeconds(5));
                    if (message != null)
                    {
                        Console.WriteLine(string.Format("Message received: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                        message.Complete();
                    }
                    else
                    {   
                        break;
                    }
                }
                catch (MessagingException e)
                {
                    message.Abandon();
                }
            }
            queueClient.Close();

            Console.ReadLine();
        }
    }
}
