using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    class InsertOperation
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");

            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("employee1");
            table.CreateIfNotExists();

            var employeeEntity = new EmployeeEntity("Kasagoni", "Shravan")
            {
                Email = "shravan.kasagoni@outlook.com",
                PhoneNumber = "123-456-7890"
            };

            var insertOpertaion = TableOperation.Insert(employeeEntity);
            table.Execute(insertOpertaion);

            Console.WriteLine("Table Storage Operations Complete, Press Any Key to Exit!");
            Console.ReadKey();
        }
    }
}
