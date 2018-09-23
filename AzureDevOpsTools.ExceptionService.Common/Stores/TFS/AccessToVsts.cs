using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public abstract class AccessToVsts
    {
        protected IApplicationInfo ApplicationInfo { get; set; }

        protected AccessToVsts(IApplicationInfo applicationInfo)
        {
            this.ApplicationInfo = applicationInfo;
        }

        public VssBasicCredential Credentials => new VssBasicCredential("", this.ApplicationInfo.PersonalAccessToken);

        protected void SendException(WorkItemJson json)
        {
            var connection = new VssConnection(new Uri(this.ApplicationInfo.AccountUri), Credentials);
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var result = workItemTrackingHttpClient.CreateWorkItemAsync(json.Json, this.ApplicationInfo.TeamProject, "Exception").Result;
        }

        public WorkItemJson CreateNewException(ExceptionEntity exception, IApplicationInfo applicationInfo)
        {
            //ensure no problem with string NG 255
            var wi = new WorkItemJson(exception, applicationInfo);

            return wi;
        }
    }
}