using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AzureDevOpsTools.Exception.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Account.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.UserMapping.Client;
using Microsoft.VisualStudio.Services.WebApi;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
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
            PersonalAccessToken = "oqmvsi6ppwxwbxjy4ytrgnrut5wuqbchhk7lw7u4voeu3dpwttwq";
            Project = "ExceptionTest";
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

        public async Task<IEnumerable<Account>> GetAccounts(string userId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes(
                            string.Format("{0}:{1}", "", this.PersonalAccessToken))));

                using (HttpResponseMessage response = await client.GetAsync(
                            $"https://app.vssps.visualstudio.com/_apis/accounts?ownerId={userId}&api-version=4.1"))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                }
            }
            return null;

        }


    }
}