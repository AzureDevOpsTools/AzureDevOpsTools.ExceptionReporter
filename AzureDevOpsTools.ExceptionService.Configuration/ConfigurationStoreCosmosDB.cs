using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public class ConfigurationStoreCosmosDB : IConfigurationStore
    {
        private DocumentClient client;
        private readonly Uri accountUri;
        private readonly Uri userUri;

        public ConfigurationStoreCosmosDB(string endpointUri, string primaryKey)
        {
            this.client = new DocumentClient(new Uri(endpointUri), primaryKey);
            this.accountUri = UriFactory.CreateDocumentCollectionUri("Configuration", "Accounts");
            this.userUri = UriFactory.CreateDocumentCollectionUri("Configuration", "Users");
        }

        public async Task CreateOrUpdateConfiguration(AccountConfiguration account)
        {
            await this.client.UpsertDocumentAsync(accountUri, account);
        }

        public AccountConfiguration GetConfiguration(string userId)
        {
            return client.CreateDocumentQuery<AccountConfiguration>(accountUri)
                .Where(c => c.Id == userId)
                .Take(1).AsEnumerable().SingleOrDefault();
        }

        public string GetUserByApiKey(string apiKey)
        {
            var userAccount = client.CreateDocumentQuery<UserAccount>(userUri)
                .Where(c => c.ApiKey == apiKey)
                .Take(1).AsEnumerable().SingleOrDefault();

            return userAccount != null ? userAccount.Id: string.Empty;
        }

        public string GetApiKey(string userId)
        {
            var userAccount = client.CreateDocumentQuery<UserAccount>(userUri)
                .Where(c => c.Id == userId)
                .Take(1).AsEnumerable().SingleOrDefault();

            return userAccount != null ? userAccount.ApiKey : string.Empty;
        }

        public async Task SetApiKey(string userId, string apiKey)
        {
            var userAccount = new UserAccount()
            {
                Id = userId,
                ApiKey = apiKey
            };

            await this.client.UpsertDocumentAsync(userUri, userAccount);
        }
    }
}
