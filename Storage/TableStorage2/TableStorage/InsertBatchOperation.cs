using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    class InsertBatchOperation
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("employee1");
            table.CreateIfNotExists();

            var employeeEntity1 = new EmployeeEntity("Kasagoni", "Person1")
            {
                Email = "email1@outlook.com",
                PhoneNumber = "123-456-7890"
            };

            var employeeEntity2 = new EmployeeEntity("Kasagoni", "Person2")
            {
                Email = "email2@outlook.com",
                PhoneNumber = "123-456-7890"
            };

            var employeeEntity3 = new EmployeeEntity("Kasagoni", "Person3")
            {
                Email = "email3@outlook.com",
                PhoneNumber = "123-456-7890"
            };

            var batchOpertaion = new TableBatchOperation();
            batchOpertaion.Insert(employeeEntity1);
            batchOpertaion.Insert(employeeEntity2);
            batchOpertaion.Insert(employeeEntity3);
            
            table.ExecuteBatch(batchOpertaion);

            Console.WriteLine("Table Storage Operations Complete, Press Any Key to Exit!");
            Console.ReadKey();
        }
    }
}
