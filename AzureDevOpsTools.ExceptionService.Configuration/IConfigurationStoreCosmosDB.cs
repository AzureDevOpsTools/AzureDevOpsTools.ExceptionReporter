using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public interface IConfigurationStore
    {
        Task CreateOrUpdateConfiguration(AccountConfiguration configuration);
        AccountConfiguration GetConfiguration(string userId);
    }
}