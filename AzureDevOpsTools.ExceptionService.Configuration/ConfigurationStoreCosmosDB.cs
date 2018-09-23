using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public class ConfigurationStoreCosmosDB : IConfigurationStore
    {
        private const string EndpointUri = "https://exceptionreporter.documents.azure.com:443/";
        private const string PrimaryKey = "yVF6eE0bzNZ22V4Ygi0Qd8LlPaQJQGq1N8fQAPvf62Cr9thbUUiEWuUp9NvgcZHFVmeTp0AsjjsAJYnFNYKaow==";
        private DocumentClient client;

        public async Task CreateConfiguration(string userId)
        {
            var config = new AccountConfiguration()
            {
                UserId = userId,
                AzureDevOpsServicesAccountUrl = "https:/dev.azure.com/whateveryousay",
                TeamProject = "ExceptionTest",
                TargetAreaPath = "ExceptionTest",
                AccessToken = "fvyhrnwxewehe2nyak6bylm3xpssot2k2iz5rkeabkik36ng776q"
            };

            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("Configuration", "Accounts"), config);
        }

        public async Task<IEnumerable<AccountConfiguration>> GetConfigurations(string userId)
        {
            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

            var query = client.CreateDocumentQuery<AccountConfiguration>(
                UriFactory.CreateDocumentCollectionUri("Configuration", "Accounts"))
                .Where(c => c.UserId == userId)
                .AsDocumentQuery();

            var results = new List<AccountConfiguration>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<AccountConfiguration>());
            }

            return results;
        }
    }
}
