using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using PizzaOrderCreator;

namespace PizzaOrderReceiver
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string queueName = "PizzaOrders";

            var connectionString = CloudConfigurationManager.GetSetting("PizzaOrderConnectionString");

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            while (true)
            {
                Console.WriteLine("Receiving...");

                BrokeredMessage message = client.Receive(TimeSpan.FromSeconds(1));

                if (message != null)
                {
                    try
                    {
                        PizzaOrder order = message.GetBody<PizzaOrder>();

                        Console.WriteLine("Cooking {0} for {1}.", order.Type, order.CustomerName);
                        Thread.Sleep(1000);
                        Console.WriteLine("{0} pizza for {1} is ready", order.Type, order.CustomerName);
                        
                        message.Complete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex.Message);
                        message.Abandon();
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
