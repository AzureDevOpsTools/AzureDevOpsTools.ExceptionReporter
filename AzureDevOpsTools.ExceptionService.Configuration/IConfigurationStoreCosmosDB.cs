using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public interface IConfigurationStore
    {
        Task CreateOrUpdateConfiguration(AccountConfiguration configuration);
        Task<AccountConfiguration> GetConfiguration(string userId);

        Task<string> GetUserByApiKey(string apiKey);
        Task<string> GetApiKey(string userId);
        Task SetApiKey(string userId, string apiKey);
    }
}