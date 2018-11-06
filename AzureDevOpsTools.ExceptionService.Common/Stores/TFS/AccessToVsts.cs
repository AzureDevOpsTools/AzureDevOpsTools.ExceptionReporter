using System;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

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

        protected void SendException(JsonPatchDocument json, string workItemType)
        {
            var connection = new VssConnection(new Uri(this.ApplicationInfo.AccountUri), Credentials);
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var result = workItemTrackingHttpClient.CreateWorkItemAsync(json, this.ApplicationInfo.TeamProject, workItemType).Result;
        }

    }
}