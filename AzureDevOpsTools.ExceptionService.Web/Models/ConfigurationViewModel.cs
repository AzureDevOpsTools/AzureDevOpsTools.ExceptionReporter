using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AzureDevOpsTools.ExceptionService.Web.Models
{
    public class ConfigurationViewModel
    {
        [Required]
        [DisplayName("Azure DevOps Account Url (eg. http://dev.azure.com/{your organization}")]
        [DataType(DataType.Url)]
        public string AccountUrl { get; set; }

        [Required]
        [DisplayName("Team Project name")]
        public string TeamProject { get; set; }

        [Required]
        [DisplayName("Area Path")]
        public string AreaPath { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PersonalAccessToken { get; set; }

        [Required]
        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }

        [ReadOnly(true)]
        [DisplayName("Api key (Generated)")]
        public string ApiKey { get; set; }
    }
}
