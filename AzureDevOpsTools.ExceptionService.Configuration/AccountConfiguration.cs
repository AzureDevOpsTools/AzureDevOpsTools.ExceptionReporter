using Newtonsoft.Json;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public class AccountConfiguration
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        public string AzureDevOpsServicesAccountUrl { get; set; }
        public string TeamProject { get; set; }
        public string PersonalAccessToken { get; set; }
        public string TargetAreaPath { get; set; }
        public string AssignedTo { get; set; }
    }

    public class UserAccount
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ApiKey { get; set; }
    }
    }