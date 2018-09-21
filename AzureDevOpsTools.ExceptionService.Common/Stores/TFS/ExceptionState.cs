using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    internal class ExceptionState
    {
        //Workitem-states:
        private const string ClosedStateName = "Done";
        private const string RemovedStateName = "Removed";
        private const string MergeStateName = "Merge";

       private readonly WorkItem workItem;

        public ExceptionState(WorkItem workItem)
        {
            this.workItem = workItem;
        }

        [ContractInvariantMethod]
// ReSharper disable UnusedMember.Local
        private void EnsureWorkItem()
// ReSharper restore UnusedMember.Local
        {
           
        }

        //public bool IsFixedAfterChangeset(int changeSetId)
        //{
        //    var associatedChangesets = GetAssociatedChangeSets();
        //    return associatedChangesets.Any(changeset => changeset.ChangesetId > changeSetId);
        //}

        //public bool HasChangeset()
        //{
        //    var associatedChangesets = GetAssociatedChangeSets();
        //    return associatedChangesets.Any();
        //}

        public bool IsOpen => !workItem.State().Equals(RemovedStateName) && !workItem.State().Equals(ClosedStateName) &&
                              !workItem.State().Equals(MergeStateName);

        // utkommentert fordi denne varianten gir contracts warnings
        //private IEnumerable<Changeset> GetAssociatedChangeSets()
        //{
        //    var externalLinks = workItem.Links.OfType<ExternalLink>();
        //    return from externalLink in externalLinks let artifact = LinkingUtilities.DecodeUri(externalLink.LinkedArtifactUri) where String.Equals(artifact.ArtifactType, "Changeset", StringComparison.Ordinal) select versionControlServer.ArtifactProvider.GetChangeset(new Uri(externalLink.LinkedArtifactUri));
        //}
       
    }
}