using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    class UpsertOperation
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("employee");

            var retrieveOperation = TableOperation.Retrieve<EmployeeEntity>("Kasagoni", "Shravan");
            var retrievedResult = table.Execute(retrieveOperation);
            
            var updateEntity = (EmployeeEntity)retrievedResult.Result;

            if (updateEntity != null)
            { 
                updateEntity.PhoneNumber = "111-111-1111";
                var updateOperation = TableOperation.Replace(updateEntity);
                table.Execute(updateOperation);

                Console.WriteLine("Entity updated.");
            }


            if (updateEntity != null)
            {   
                updateEntity.PhoneNumber = "123-455-4321";
                var insertOrReplaceOperation = TableOperation.InsertOrReplace(updateEntity);
                table.Execute(insertOrReplaceOperation);
                Console.WriteLine("Entity was updated.");
            }


            Console.WriteLine("Table Storage Operations Complete, Press Any Key to Exit!");
            Console.ReadKey();
        }
    }
}