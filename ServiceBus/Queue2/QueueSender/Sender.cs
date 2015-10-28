using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System;
using System.Threading;

namespace QueueSender
{
    class Sender
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            var nameSpaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!nameSpaceManager.QueueExists("MySampleQueue2"))
            {
                nameSpaceManager.CreateQueue("MySampleQueue2");
            }

            QueueClient queueClient = QueueClient.CreateFromConnectionString(connectionString, "MySampleQueue2");
            
            Console.WriteLine("\nSending messages to Queue...");

            int number = 1;

            while (true)
            {
              
                var message = CreateBrokeredMessage(number.ToString(), "Message Information ");
                try
                {
                    queueClient.Send(message);
                }
                catch (MessagingException e)
                {
                    throw e;        
                }
                Console.WriteLine(string.Format("Message sent: Id = {0}, Body = {1}", message.MessageId, message.GetBody<string>()));
                Thread.Sleep(5000);
                number++;
                if (number == 100)
                    break;
            }
        }

        private static BrokeredMessage CreateBrokeredMessage(string messageId, string messageBody)
        {
            BrokeredMessage message = new BrokeredMessage(messageBody + messageId);
            message.MessageId = messageId;
            return message;
        }
    }
}
