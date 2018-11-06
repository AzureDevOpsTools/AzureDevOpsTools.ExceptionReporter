using AzureDevOpsTools.ExceptionService.Common;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;

namespace AzureDevOpsTools.Exception.Common.Stores.TFS
{
    public class TFSStoreWithBug : AccessToVsts
    {
        public TFSStoreWithBug(IApplicationInfo applicationInfo)
            : base(applicationInfo)
        {
        }


        public BugWorkItemJson CreateNewException(ExceptionEntity exception, IApplicationInfo applicationInfo)
        {
            //ensure no problem with string NG 255
            var wi = new BugWorkItemJson(exception, applicationInfo);

            return wi;
        }

        public void RegisterException(ExceptionEntity exceptionEntity, IApplicationInfo applicationInfo)
        {
            var workItem = CreateNewException(exceptionEntity, applicationInfo);
            SendException(workItem.Json, "Bug");

        }

    }
}
