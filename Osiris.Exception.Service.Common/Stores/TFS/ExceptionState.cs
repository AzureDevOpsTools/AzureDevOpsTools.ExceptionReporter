using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace Inmeta.Exception.Service.Common.Stores.TFS
{
    internal class ExceptionState
    {
        //Workitem-states:
        private const string ClosedStateName = "Closed";
        private const string ResolvedStateName = "Resolved";
        private const string MergeStateName = "Merge";

        private readonly VersionControlServer versionControlServer;
        private readonly WorkItem workItem;

        public ExceptionState(WorkItem workItem, VersionControlServer versionControlServer)
        {
            Contract.Requires(workItem != null);
            Contract.Requires(workItem.Links != null);
            Contract.Requires(versionControlServer != null);

            this.versionControlServer = versionControlServer;
            this.workItem = workItem;
        }

        [ContractInvariantMethod]
// ReSharper disable UnusedMember.Local
        private void EnsureWorkItem()
// ReSharper restore UnusedMember.Local
        {
            Contract.Invariant(workItem != null);
            Contract.Invariant(workItem.Links != null);
        }

        public bool IsFixedAfterChangeset(int changeSetId)
        {
            var associatedChangesets = GetAssociatedChangeSets();
            return associatedChangesets.Any(changeset => changeset.ChangesetId > changeSetId);
        }

        public bool IsOpen
        {
            get
            {
                return !workItem.State.Equals(ResolvedStateName) && !workItem.State.Equals(ClosedStateName) &&
                       !workItem.State.Equals(MergeStateName);
            }
        }

        // utkommentert fordi denne varianten gir contracts warnings
        private IEnumerable<Changeset> GetAssociatedChangeSets()
        {
            Contract.Ensures(Contract.Result<IEnumerable<Changeset>>() != null);

            var externalLinks = workItem.Links.OfType<ExternalLink>();
            return from externalLink in externalLinks let artifact = LinkingUtilities.DecodeUri(externalLink.LinkedArtifactUri) where String.Equals(artifact.ArtifactType, "Changeset", StringComparison.Ordinal) select versionControlServer.ArtifactProvider.GetChangeset(new Uri(externalLink.LinkedArtifactUri));
        }
       
    }
}