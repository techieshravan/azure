using System;
using System.Collections.Generic;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableStorage
{
    public class Retrieve
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("employee");

            //Single Entity
            Console.WriteLine(Environment.NewLine + "Retrieve Single Entity");
            var retrieveOperation = TableOperation.Retrieve<EmployeeEntity>("Kasagoni", "Shravan");
            var retrievedResult = table.Execute(retrieveOperation);
            Console.WriteLine(retrievedResult.Result != null ? ((EmployeeEntity) retrievedResult.Result).Email : "The email could not be retrieved.");


            //All entities in single partition
            Console.WriteLine(Environment.NewLine + "Retrieve All entities in single partition");
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Kasagoni");
            var query = new TableQuery<EmployeeEntity>().Where(filter);
            var entities = table.ExecuteQuery(query);

            foreach (var entity in entities)
            {
                Console.WriteLine("{0}, {1} {2} {3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
            }


            //filter entities in single partition
            Console.WriteLine(Environment.NewLine + "filter entities in single partition");
            var filter1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Kasagoni");
            var filter2 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "S");
            var filterCombination = TableQuery.CombineFilters(filter1, TableOperators.And, filter2);
            var rangeQuery = new TableQuery<EmployeeEntity>().Where(filterCombination);
            
            foreach (var entity in table.ExecuteQuery(rangeQuery))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
            }


            //Query a subset of entity properties
            Console.WriteLine(Environment.NewLine + "Query a subset of entity properties");

            var projectionQuery1 = new TableQuery<DynamicTableEntity>().Select(new string[] { "PhoneNumber" });
            EntityResolver<string> resolver1 = (pk, rk, ts, props, etag) => props.ContainsKey("PhoneNumber") ? props["PhoneNumber"].StringValue : null;
            
            var projectionResult1 = table.ExecuteQuery(projectionQuery1, resolver1);

            foreach (var projectedPhoneNumber in projectionResult1)
            {
                Console.WriteLine(projectedPhoneNumber);
            }


            var projectionQuery2 = new TableQuery<DynamicTableEntity>().Select(new string[] { "PhoneNumber", "Email" });
            
            EntityResolver<EmployeeEntity> resolver2 = (pk, rk, ts, props, etag) =>
            {
                var item = new EmployeeEntity
                {
                    PartitionKey = pk,
                    RowKey = rk,
                    Timestamp = ts,
                    ETag = etag,
                    PhoneNumber = props.ContainsKey("PhoneNumber") ? props["PhoneNumber"].StringValue : null,
                    Email = props.ContainsKey("Email") ? props["Email"].StringValue : null
                };

                return item;
            };

            var projectionResult2 = table.ExecuteQuery(projectionQuery2, resolver2);

            foreach (var entity in projectionResult2)
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}\t{4}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber,entity.ETag);
            }

            Console.WriteLine("Table Storage Operations Complete, Press Any Key to Exit!");
            Console.ReadKey();
        }
    }
}
