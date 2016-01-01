using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using System;

namespace MyFixIt.Persistence
{
    static class StorageUtils
    {
        public static CloudStorageAccount StorageAccount
        {
            get
            {
                string account = CloudConfigurationManager.GetSetting("StorageAccountName");
                // This enables the storage emulator when running locally using the Azure compute emulator.
                if (account == "{StorageAccountName}")
                {
                    return CloudStorageAccount.DevelopmentStorageAccount;
                }

                string key = CloudConfigurationManager.GetSetting("StorageAccountAccessKey");
                string connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
                return CloudStorageAccount.Parse(connectionString);
            }
        }
    }
}
