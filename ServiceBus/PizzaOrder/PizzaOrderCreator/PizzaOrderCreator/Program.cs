using System;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace PizzaOrderCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string queueName = "PizzaOrders";

            var connectionString = CloudConfigurationManager.GetSetting("PizzaOrderConnectionString");

            var nameSpaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!nameSpaceManager.QueueExists(queueName))
            {
                nameSpaceManager.CreateQueue(queueName);
            }

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            var pizzaTypes = new List<string>
            {
                "Margherita",
                "Funghi",
                "Capricciosa",
                "Quattro Stagioni",
                "Vegetariana",
                "Quattro Formaggi",
                "Marinara",
                "Peperoni",
                "Napolitana",
                "Hawaii",
                "Maltija (Maltese)",
                "Calzone (folded)",
                "Rucola",
                "Bolognese",
                "Meat Feast",
                "Kebabpizza",
                "Mexicana"
            };

            var pizzaSizes = new List<string>
            {
                "Small",
                "Medium",
                "Large",
                "Extra Large"
            };

            var random = new Random();

            for (var i = 1; i <= 100; i++)
            {
                var order = new PizzaOrder
                {
                    CustomerName = "Shravan" + i,
                    Type = pizzaTypes[random.Next(pizzaTypes.Count)],
                    Size = pizzaSizes[random.Next(pizzaSizes.Count)],
                    Quantity = random.Next(10)
                };

                var message = new BrokeredMessage(order)
                {
                    Label = "PizzaOrder"
                };

                Console.WriteLine("Message size: " + message.Size);

                client.SendAsync(message);

                //System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine("Sending Messages Complete");

            Console.ReadLine();
        }
    }
}
