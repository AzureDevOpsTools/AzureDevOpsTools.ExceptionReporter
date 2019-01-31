using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public class AccountConfiguration : TableEntity
    {
        public AccountConfiguration()
        {

        }
        public AccountConfiguration(string id)
        {
            this.PartitionKey = id;
            this.RowKey = id;
        }
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        public string AzureDevOpsServicesAccountUrl { get; set; }
        public string TeamProject { get; set; }
        public string PersonalAccessToken { get; set; }
        public string TargetAreaPath { get; set; }
        public string AssignedTo { get; set; }
        public bool UseExceptionWorkItemType { get; set; }
    }

    public class UserAccount : TableEntity
    {
        public UserAccount()
        {

        }
        public UserAccount(string id)
        {
            this.PartitionKey = id;
            this.RowKey = id;
        }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ApiKey { get; set; }
    }
    }