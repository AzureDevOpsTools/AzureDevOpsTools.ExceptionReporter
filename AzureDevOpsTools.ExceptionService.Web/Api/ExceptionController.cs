using AzureDevOpsTools.Exception.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Common;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AzureDevOpsTools.ExceptionService.Web
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class ExceptionController : ControllerBase
    {
        private readonly IConfigurationStore configuration;

        public ExceptionController(IConfigurationStore configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExceptionEntity exception)
        {
            //TODO: Move this check into a custom middleware handler
            var apiKey = Request.Headers["X-ApiKey"];
            if( string.IsNullOrEmpty(apiKey))
                return Unauthorized();

            var userId = this.configuration.GetUserByApiKey(apiKey);
            if( string.IsNullOrEmpty(userId))
                return Unauthorized();

            var registrator = new TfsStoreWithException();
            //registrator = new TFSStoreWithBug();
            var configuration = this.configuration.GetConfiguration(userId);
            var settings = new ExceptionSettings(exception.ApplicationName);
            settings.Area = configuration.TargetAreaPath;
            settings.AssignedTo = configuration.AssignedTo;
            settings.TeamProject = configuration.TeamProject;
            settings.TfsServer = configuration.AzureDevOpsServicesAccountUrl;

            registrator.RegisterException(exception, settings);

            return Ok();
        }

        [HttpGet]
        public string Get()
        {
            return "Hello World!";
        }

    }
}
