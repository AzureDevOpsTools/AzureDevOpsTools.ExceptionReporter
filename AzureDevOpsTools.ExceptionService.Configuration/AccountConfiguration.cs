namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public class AccountConfiguration
    {
        public string UserId { get; set; }
        public string AzureDevOpsServicesAccountUrl { get; set; }
        public string TeamProject { get; set; }
        public string AccessToken { get; set; }
        public string TargetAreaPath { get; set; }
    }
}