using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Inmeta.Exception.Service.Common;

namespace Inmeta.Exception.Reporter.Web.Models
{

    public class FileNameAndItemsViewModel
    {
        public string FileName { get; set; }
        public IList<ExceptionEntity> Exceptions { get; set; }
    }
}