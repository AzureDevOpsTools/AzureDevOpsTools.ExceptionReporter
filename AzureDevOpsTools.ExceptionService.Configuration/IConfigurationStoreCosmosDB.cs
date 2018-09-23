using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Configuration
{
    public interface IConfigurationStore
    {
        Task CreateConfiguration(string userId);
        Task<IEnumerable<AccountConfiguration>> GetConfigurations(string userId);
    }
}