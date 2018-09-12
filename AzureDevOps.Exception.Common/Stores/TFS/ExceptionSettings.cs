namespace AzureDevOps.Exception.Service.Common.Stores.TFS
{
 
    public class ExceptionSettings : IApplicationInfo
    {
        private readonly string applicationName;

        public ExceptionSettings(string applicationName)
        {   
            this.applicationName = applicationName;

            this.TfsServer = "https://whateveryousay.visualstudio.com";
            this.Collection = "DefaultCollection";
            this.TeamProject = "ExceptionTest";
            this.Area = "ExceptionTest";
            this.AssignedTo = "jakob@ehn.nu";
        }

        public string ApplicationName { get; private set; }
        public string TfsServer { get; private set; }
        public string Collection { get; private set; }
        public string TeamProject{ get; private set; }
        public string Area { get; private set; }
        public string AssignedTo { get; private set; }
    }
}