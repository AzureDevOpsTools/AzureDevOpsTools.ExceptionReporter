namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
 
    public class ExceptionSettings : IApplicationInfo
    {
        private readonly string applicationName;

        public ExceptionSettings(string applicationName)
        {   
            this.applicationName = applicationName;

            this.TfsServer = "https://whateveryousay.visualstudio.com";
  //          this.Collection = "DefaultCollection";
            this.TeamProject = "ExceptionTest";
            this.Area = "ExceptionTest";
            this.AssignedTo = "jakob@ehn.nu";
        }

        public string ApplicationName { get; set; }
        public string TfsServer { get; set; }
//        public string Collection { get; set; }
        public string TeamProject{ get; set; }
        public string Area { get; set; }
        public string AssignedTo { get; set; }
    }
}