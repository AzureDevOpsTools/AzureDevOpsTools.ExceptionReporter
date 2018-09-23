using AzureDevOpsTools.ExceptionService.Configuration;
using AzureDevOpsTools.ExceptionService.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Web.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationStore configuration;

        public ConfigurationController(IConfigurationStore configuration)
        {
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var model = configuration.GetConfiguration(userId);

            if(model != null)
            {
                var viewModel = new ConfigurationViewModel()
                {
                    AccountUrl = model.AzureDevOpsServicesAccountUrl,
                    TeamProject = model.TeamProject,
                    AreaPath = model.TargetAreaPath,
                };
                return View(viewModel);
            }
            return View();
        }

        public async Task<IActionResult> Post(ConfigurationViewModel model)
        {
            if( !ModelState.IsValid)
                return View("Index");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if( model.PersonalAccessToken == "dummy_password")
            {
                var existing = this.configuration.GetConfiguration(userId);
                if( existing != null)
                {
                    model.PersonalAccessToken = existing.PersonalAccessToken;
                }
            }

            var config = new AccountConfiguration()
            {
                AzureDevOpsServicesAccountUrl = model.AccountUrl,
                TargetAreaPath = model.AreaPath,
                TeamProject = model.TeamProject,
                PersonalAccessToken = model.PersonalAccessToken,
                Id = userId
            };
            await this.configuration.CreateOrUpdateConfiguration(config);

            return RedirectToAction("Index");
        }
    }
}
