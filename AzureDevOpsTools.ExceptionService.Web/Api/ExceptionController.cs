using AzureDevOpsTools.Exception.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Common;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AzureDevOpsTools.ExceptionService.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        private readonly IConfigurationStore configuration;

        public ExceptionController(IConfigurationStore configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public void Post([FromBody] ExceptionEntity exception)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var registrator = new TfsStoreWithException();
            //registrator = new TFSStoreWithBug();
            var configuration = this.configuration.GetConfiguration(userId);
            var settings = new ExceptionSettings(exception.ApplicationName);
            settings.Area = configuration.TargetAreaPath;
            settings.AssignedTo = configuration.AssignedTo;
            settings.TeamProject = configuration.TeamProject;
            settings.TfsServer = configuration.AzureDevOpsServicesAccountUrl;

            registrator.RegisterException(exception, settings);
        }

        [HttpGet]
        public string Get()
        {
            return "Hello World!";
        }

    }
}
