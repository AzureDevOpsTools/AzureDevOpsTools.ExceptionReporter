using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureDevOpsTools.ExceptionService.Web.Models
{
    public class ConfigurationViewModel
    {
        [Required]
        [DataType(DataType.Url)]
        public string AccountUrl { get; set; }
        [Required]
        public string TeamProject { get; set; }
        [Required]
        public string AreaPath { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string PersonalAccessToken { get; set; }
    }
}
