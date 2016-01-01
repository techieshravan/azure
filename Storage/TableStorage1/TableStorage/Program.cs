using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace TableStorage
{
    public class Program
    {
      

        public static void Main(string[] args)
        {
            Console.WriteLine("Azure Storage Table Sample\n");

            var tableStorageOperations = new TableStorageOperations();
            
            CloudTable table = tableStorageOperations.CreateTableAsync().Result;

            tableStorageOperations.BasicTableOperationsAsync(table).Wait();

            tableStorageOperations.AdvancedTableOperationsAsync(table).Wait();
            
            Console.WriteLine("Press any key to exit");

            Console.Read();
        }

      
    }
}