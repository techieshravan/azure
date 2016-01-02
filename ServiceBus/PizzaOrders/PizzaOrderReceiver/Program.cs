using System;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace PizzaOrderReceiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string queueName = "PizzaOrders";

            var connectionString = CloudConfigurationManager.GetSetting("PizzaOrderConnectionString");

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            //var client = QueueClient.CreateFromConnectionString(connectionString, queueName, ReceiveMode.ReceiveAndDelete);
            //var client = QueueClient.CreateFromConnectionString(connectionString, queueName, ReceiveMode.PeekLock);

            while (true)
            {
                Console.WriteLine("Receiving...");

                BrokeredMessage message = client.Receive(TimeSpan.FromSeconds(1));

                if (message != null)
                {
                    if (message.ContentType.Equals(("application/json")))
                    {
                        try
                        {
                            string content = message.GetBody<string>();

                            if (message.Label.Equals("PizzaOrders.MessageContracts.PizzaOrder"))
                            {
                                dynamic order = JsonConvert.DeserializeObject(content);

                                Console.WriteLine("Cooking {0} for {1}.", order.Type, order.CustomerName);
                                Thread.Sleep(1000);
                                Console.WriteLine("{0} pizza for {1} is ready", order.Type, order.CustomerName);

                                message.Complete();
                            }
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine("Exception: " + ex.Message);
                            message.Abandon();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No message prsent on queue.");
                }
            }
        }
    }
}