using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    class DeleteOperation
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("employee");
            
            var retrieveOperation = TableOperation.Retrieve<EmployeeEntity>("Kasagoni", "Person2");
            var retrievedResult = table.Execute(retrieveOperation);
            var deleteEntity = (EmployeeEntity)retrievedResult.Result;

            if (deleteEntity != null)
            {
                var deleteOperation = TableOperation.Delete(deleteEntity);
                table.Execute(deleteOperation);
                Console.WriteLine("Entity deleted.");
            }
            else
            {
                Console.WriteLine("Could not retrieve the entity.");
            }   

            Console.WriteLine("Table Storage Operations Complete, Press Any Key to Exit!");
            Console.ReadKey();
        }
    }
}
