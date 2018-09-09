using System.Collections.Generic;
using AzureDevOps.Exception.Service.Common;

namespace AzureDevOps.Exception.Reporter.Web.Models
{

    public class FileNameAndItemsViewModel
    {
        public string FileName { get; set; }
        public IList<ExceptionEntity> Exceptions { get; set; }
    }
}