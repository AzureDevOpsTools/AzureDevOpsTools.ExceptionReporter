
namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public interface IApplicationInfo
    {
        string ApplicationName { get; }
        string AccountUri { get; }
        string TeamProject { get; }
        string Area { get; }
        string AssignedTo { get; }
        string PersonalAccessToken { get; }
    }
}
