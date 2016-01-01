using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    class RetrieveAsynchronously
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("employee1");
            table.CreateIfNotExists();


            //for (int i = 1; i <= 5000; i++)
            //{
            //    var employeeEntity = new EmployeeEntity("Kasagoni", "Name" + i)
            //    {
            //        Email = "email" + i + "@outlook.com",
            //        PhoneNumber = "123-456-7890"
            //    };

            //    var insertOpertaion = TableOperation.Insert(employeeEntity);
            //    table.Execute(insertOpertaion);
            //}
            
            GetDataAync(table).Wait();
            
            Console.WriteLine("Table Storage Operations Complete, Press Any Key to Exit!");
            Console.ReadKey();
        }

        private static async Task GetDataAync(CloudTable table)
        {
            var tableQuery = new TableQuery<EmployeeEntity>();

            TableContinuationToken continuationToken = null;

            do
            {
                TableQuerySegment<EmployeeEntity> tableQueryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                Console.WriteLine("Rows retrieved {0}", tableQueryResult.Results.Count);
                
            } while (continuationToken != null);

        }
    }
}
