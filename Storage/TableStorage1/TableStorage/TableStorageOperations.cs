using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableStorage.Model;

namespace TableStorage
{
    class TableStorageOperations
    {
        private const string TableName = "person";

        public async Task<CloudTable> CreateTableAsync()
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine("1. Create a Table for the demo");

            CloudTable table = tableClient.GetTableReference(TableName);

            try
            {
                if (await table.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Created Table named: {0}", TableName);
                }
                else
                {
                    Console.WriteLine("Table {0} already exists", TableName);
                }
            }
            catch (StorageException ex)
             {
                Console.WriteLine(ex);
                Console.ReadLine();
                throw;
            }

            return table;
        }

        public async Task BasicTableOperationsAsync(CloudTable table)
        {
            PersonEntity customer = new PersonEntity("Harp", "Walter")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };

            Console.WriteLine("2. Update an existing Entity using the InsertOrMerge Upsert Operation.");
            customer.PhoneNumber = "425-555-0105";
            customer = await InsertOrMergeEntityAsync(table, customer);

            Console.WriteLine("3. Reading the updated Entity.");
            customer = await RetrieveEntityUsingPointQueryAsync(table, "Harp", "Walter");

            Console.WriteLine("4. Delete the entity. ");
            await DeleteEntityAsync(table, customer);
        }

        public async Task AdvancedTableOperationsAsync(CloudTable table)
        {
            Console.WriteLine("4. Inserting a batch of entities. ");
            await BatchInsertOfCustomerEntitiesAsync(table);

            Console.WriteLine("5. Retrieving entities with surname of Smith and first names >= 1 and <= 75");
            await PartitionRangeQueryAsync(table, "Smith", "0001", "0075");

            Console.WriteLine("6. Retrieve entities with surname of Smith.");
            await PartitionScanAsync(table, "Smith");
        }

        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

        private async Task<PersonEntity> InsertOrMergeEntityAsync(CloudTable table, PersonEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            PersonEntity insertedCustomer = result.Result as PersonEntity;
            return insertedCustomer;
        }

        private async Task<PersonEntity> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<PersonEntity>(partitionKey, rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            PersonEntity customer = result.Result as PersonEntity;
            if (customer != null)
            {
                Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", customer.PartitionKey, customer.RowKey, customer.Email, customer.PhoneNumber);
            }

            return customer;
        }

        private async Task DeleteEntityAsync(CloudTable table, PersonEntity deleteEntity)
        {
            if (deleteEntity == null)
            {
                throw new ArgumentNullException("deleteEntity");
            }

            TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
            await table.ExecuteAsync(deleteOperation);
        }

        private async Task BatchInsertOfCustomerEntitiesAsync(CloudTable table)
        {
            TableBatchOperation batchOperation = new TableBatchOperation();

            for (int i = 0; i < 100; i++)
            {
                batchOperation.InsertOrMerge(new PersonEntity("Smith", string.Format("{0}", i.ToString("D4")))
                {
                    Email = string.Format("{0}@contoso.com", i.ToString("D4")),
                    PhoneNumber = string.Format("425-555-{0}", i.ToString("D4"))
                });
            }


            IList<TableResult> results = await table.ExecuteBatchAsync(batchOperation);

            foreach (var res in results)
            {
                var customerInserted = res.Result as PersonEntity;
                Console.WriteLine("Inserted entity with\t Etag = {0} and PartitionKey = {1}, RowKey = {2}", customerInserted.ETag, customerInserted.PartitionKey, customerInserted.RowKey);
            }

        }

        private async Task PartitionRangeQueryAsync(CloudTable table, string partitionKey, string startRowKey, string endRowKey)
        {

            TableQuery<PersonEntity> rangeQuery = new TableQuery<PersonEntity>().Where(
                TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                        TableOperators.And,
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, startRowKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, endRowKey))));


            TableContinuationToken token = null;
            rangeQuery.TakeCount = 50;
            do
            {
                TableQuerySegment<PersonEntity> segment = await table.ExecuteQuerySegmentedAsync(rangeQuery, token);
                token = segment.ContinuationToken;
                foreach (PersonEntity entity in segment)
                {
                    Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
                }
            }
            while (token != null);
        }

        private async Task PartitionScanAsync(CloudTable table, string partitionKey)
        {
            TableQuery<PersonEntity> partitionScanQuery = new TableQuery<PersonEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<PersonEntity> segment = await table.ExecuteQuerySegmentedAsync(partitionScanQuery, token);
                token = segment.ContinuationToken;
                foreach (PersonEntity entity in segment)
                {
                    Console.WriteLine("Customer: {0},{1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey, entity.Email, entity.PhoneNumber);
                }
            }
            while (token != null);
        }

        private async Task DeleteTableAsync(CloudTable table)
        {
            await table.DeleteIfExistsAsync();
        }
    }
}
