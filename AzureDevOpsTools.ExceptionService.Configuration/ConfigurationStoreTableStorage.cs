using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public class ConfigurationStoreTableStorage : IConfigurationStore
    {
        private readonly string storageConnectionString;
        private readonly CloudTable usersTable;
        private readonly CloudTable accountsTable;

        public ConfigurationStoreTableStorage(string storageConnectionString)
        {
            this.storageConnectionString = storageConnectionString;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            accountsTable = tableClient.GetTableReference("Accounts");
            usersTable = tableClient.GetTableReference("Users");
            bool exists = accountsTable.CreateIfNotExistsAsync().Result;
            exists = usersTable.CreateIfNotExistsAsync().Result;

        }

        public async Task CreateOrUpdateConfiguration(AccountConfiguration configuration)
        {
            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.InsertOrReplace(configuration);

            // Execute the insert operation.
            await accountsTable.ExecuteAsync(insertOperation);
        }

        public async Task<string> GetApiKey(string userId)
        {
            TableOperation getOperation = TableOperation.Retrieve<UserAccount>(userId, userId);
            var config = (await usersTable.ExecuteAsync(getOperation)).Result as UserAccount;
            if( config != null)
                return config.ApiKey;
            return null;
        }

        public async Task<AccountConfiguration> GetConfiguration(string userId)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<AccountConfiguration>(userId, userId);
            TableResult result = await accountsTable.ExecuteAsync(retrieveOperation);
            return result.Result as AccountConfiguration;
        }

        public async Task<string> GetUserByApiKey(string apiKey)
        {
            var query = new TableQuery<UserAccount>();
            TableContinuationToken token = null;
            var users = await usersTable.ExecuteQuerySegmentedAsync(query, token);
            var user = users.Results.FirstOrDefault(u => u.ApiKey == apiKey);
            if( user != null)
                return user.PartitionKey;
            return null;
        }

        public async Task SetApiKey(string userId, string apiKey)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(
                new UserAccount(userId){ApiKey = apiKey });
            await usersTable.ExecuteAsync(insertOperation);

        }
    }
}
