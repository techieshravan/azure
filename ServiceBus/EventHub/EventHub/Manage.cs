using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace EventHub
{
    public class Manage
    {
        public static void CreateEventHub(string eventHubName, int numberOfPartitions, NamespaceManager manager)
        {
            try
            {
                // Create the Event Hub
                Console.WriteLine("Creating Event Hub...");
                var ehd = new EventHubDescription(eventHubName)
                {
                    PartitionCount = numberOfPartitions
                };
                manager.CreateEventHubIfNotExistsAsync(ehd).Wait();
            }
            catch (AggregateException agexp)
            {
                Console.WriteLine(agexp.Flatten());
            }
        }


        public static async Task<EventHubDescription> UpdateEventHub(string eventHubName, NamespaceManager namespaceManager)
        {
            // Add a consumer group
            var ehd = await namespaceManager.GetEventHubAsync(eventHubName);
            await namespaceManager.CreateConsumerGroupIfNotExistsAsync(ehd.Path, "consumerGroupName");

            // Create a customer SAS rule with Manage permissions
            ehd.UserMetadata = "Some updated info";
            var ruleName = "myeventhubmanagerule";
            var ruleKey = SharedAccessAuthorizationRule.GenerateRandomKey();
            ehd.Authorization.Add(new SharedAccessAuthorizationRule(ruleName, ruleKey, new[] {AccessRights.Manage, AccessRights.Listen, AccessRights.Send}));

            var ehdUpdated = await namespaceManager.UpdateEventHubAsync(ehd);
            return ehd;
        }
    }
}