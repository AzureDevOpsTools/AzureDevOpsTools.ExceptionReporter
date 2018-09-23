namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
 
    public class ExceptionSettings : IApplicationInfo
    {
        public ExceptionSettings(string applicationName, string accountUri, string teamProject, string area, string assignedTo, string personalAccessToken)
        {   
            this.ApplicationName = applicationName;
            this.AccountUri = accountUri;
            this.TeamProject = teamProject;
            this.PersonalAccessToken = personalAccessToken;
            this.Area = area;
            this.AssignedTo = assignedTo;
        }

        public string ApplicationName { get; set; }
        public string AccountUri { get; set; }
        public string TeamProject{ get; set; }
        public string PersonalAccessToken { get; }
        public string Area { get; set; }
        public string AssignedTo { get; set; }
    }
}