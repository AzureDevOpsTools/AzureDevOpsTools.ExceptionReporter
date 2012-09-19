using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inmeta.Exception.Reporter.Web.Models;
using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Stores;
using Inmeta.Exception.Service.Common.Stores.FileStore;
using Inmeta.Exception.Service.Common.Stores.TFS;

namespace Inmeta.Exception.Reporter.Web.Controllers
{

    [HandleError]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public ActionResult Index(string filename)
        {
            return View(new FileNameAndItemsViewModel (){  Exceptions = new FileStore().ParseExcpetions(GetPath(filename)).Item1, FileName = filename});
        }

        public ActionResult Upload()
        {
            var fileName = "";
            foreach (string inputTagName in Request.Files)
            {
                var file = Request.Files[inputTagName];

                if (file != null && file.ContentLength != 0 && !String.IsNullOrEmpty(file.FileName))
                {
                    //make unique filename.
                    fileName = Path.GetFileName(file.FileName) + Path.GetRandomFileName();
                    file.SaveAs(GetPath(fileName));
                    break;
                }
            }

            return RedirectToAction("Index", new { filename = fileName });
        }

        private string GetPath(string fileName)
        {
            var path = String.IsNullOrEmpty(fileName)
                        ? String.Empty
                        : Path.Combine(Path.GetTempPath(), fileName);
            return path; 
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Commit(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return RedirectToAction("Index", new {filename = String.Empty});
            }


            //parse exceptions from file
            var exceptions = new FileStore().ParseExcpetions(GetPath(filename)).Item1;

            //send to store.
            bool storeIsTFS = true;
            bool.TryParse(ConfigurationManager.AppSettings["UseTFS"], out storeIsTFS);

            Uri serviceUri = null;
            try
            {
                serviceUri = new Uri( ConfigurationManager.AppSettings["ServiceURL"]);
            }
            catch (System.Exception)
            {
                serviceUri = null;
            }


            new ExceptionStore(serviceUri, storeIsTFS).StoreException(exceptions, HttpContext.Server.MapPath(".") + @"\..\App_Data\Applications.xml");

            try
            {
                //if all suceeded delete this file from cahche.
                System.IO.File.Delete(GetPath(filename));
            }
            catch
            {
                //this is just a clean up...ignore if failure.
            }

            return RedirectToAction("Index", new { filename = String.Empty });
        }
    }

}
