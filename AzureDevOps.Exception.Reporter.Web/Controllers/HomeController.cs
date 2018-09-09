using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using AzureDevOps.Exception.Reporter.Web.Models;
using AzureDevOps.Exception.Service.Common;
using AzureDevOps.Exception.Service.Common.Stores;
using AzureDevOps.Exception.Service.Common.Stores.FileStore;

namespace AzureDevOps.Exception.Reporter.Web.Controllers
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

                if (file != null && file.ContentLength != 0 && !string.IsNullOrEmpty(file.FileName))
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
            var path = string.IsNullOrEmpty(fileName)
                        ? string.Empty
                        : Path.Combine(Path.GetTempPath(), fileName);
            return path; 
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Commit(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return RedirectToAction("Index", new {filename = string.Empty});
            }


            //parse exceptions from file
            var exceptions = new FileStore().ParseExcpetions(GetPath(filename)).Item1;

            //send to store.
            bool.TryParse(ConfigurationManager.AppSettings["UseTFS"], out var storeIsTFS);

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

            return RedirectToAction("Index", new { filename = string.Empty });
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Test()
        {

            var te = new TestException();
            te.SendException();


            return RedirectToAction("Index", new { filename = string.Empty });
        }


    }

}
