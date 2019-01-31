using AzureDevOpsTools.Exception.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Common;
using AzureDevOpsTools.ExceptionService.Common.Stores.TFS;
using AzureDevOpsTools.ExceptionService.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Post([FromBody] ExceptionEntity ex)
        {
            //TODO: Move this check into a custom middleware handler
            var apiKey = Request.Headers["X-ApiKey"];
            if( string.IsNullOrEmpty(apiKey))
                return Unauthorized();

            var userId = await this.configuration.GetUserByApiKey(apiKey);
            if( string.IsNullOrEmpty(userId))
                return Unauthorized();

            var c = await this.configuration.GetConfiguration(userId);
            var settings = new ExceptionSettings(ex.ApplicationName, 
                c.AzureDevOpsServicesAccountUrl, 
                c.TeamProject, c.TargetAreaPath, c.AssignedTo, c.PersonalAccessToken);

            var registrator = new TfsStoreWithException(settings);
            //var registrator = new TFSStoreWithBug();
            registrator.RegisterException(ex);

            return Ok();
        }

        [HttpGet]
        public string Get()
        {
            return "Hello World!";
        }
    }
}
