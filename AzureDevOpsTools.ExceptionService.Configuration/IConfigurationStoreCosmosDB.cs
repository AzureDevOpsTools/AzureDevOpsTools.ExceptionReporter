using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public interface IConfigurationStore
    {
        Task CreateOrUpdateConfiguration(AccountConfiguration configuration);
        AccountConfiguration GetConfiguration(string userId);

        string GetUserByApiKey(string apiKey);
        string GetApiKey(string userId);
        Task SetApiKey(string userId, string apiKey);
    }
}