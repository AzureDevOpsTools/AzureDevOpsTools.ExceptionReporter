using System;
using AzureDevOps.Exception.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOps.Exception.Service.Common.Stores.TFS
{
    public abstract class AccessToVsts
    {
        public Uri Uri { get; }
        public string PersonalAccessToken { get; }
        public string Project { get; }
        protected AccessToVsts()
        {
            const string uri = "https://whateveryousay.visualstudio.com";
            Uri = new Uri(uri);
            PersonalAccessToken = "xxxx";
            Project = "Project";
        }

        public VssBasicCredential Credentials => new VssBasicCredential("", PersonalAccessToken);

        protected void SendException(WorkItemJson json)
        {
            var connection = new VssConnection(Uri, Credentials);
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                var result = workItemTrackingHttpClient.CreateWorkItemAsync(json.Json, Project, "Exception").Result;

                Console.WriteLine($"Exception Successfully Created: Exception #{result.Id}");
            }
            catch (AggregateException ex)
            {
                ServiceLog.Error($"Error creating Exception: {ex.InnerException.Message}");
            }
        }

        public WorkItemJson CreateNewException(ExceptionEntity exception, IApplicationInfo applicationInfo)
        {
            //ensure no problem with string NG 255
            var wi = new WorkItemJson(exception, applicationInfo);

            return wi;
        }
    }
}