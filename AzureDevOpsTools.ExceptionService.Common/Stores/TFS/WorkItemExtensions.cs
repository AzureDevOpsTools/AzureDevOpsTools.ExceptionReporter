using System.Collections.Generic;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public static class WorkItemExtensions
    {
        public static string State(this WorkItem wi)
        {
            var state = wi.Fields["System.State"] as string;
            return state;
        }

        public static void State(this WorkItem wi,string newstate)
        {
            wi.Fields["System.State"] = newstate;
        
        }

        public static string Field(this WorkItem wi, string fieldname)
        {
            var val = wi.Fields[fieldname] as string;
            return val;
        }

        /// <summary>
        /// Returns the errors found in workitem.
        /// </summary>
        public static IReadOnlyCollection<string> Validate(this WorkItem wi)
        {
            return new List<string>();
        }
    }
}