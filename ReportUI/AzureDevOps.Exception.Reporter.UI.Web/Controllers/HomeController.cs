﻿using System.Web.Mvc;

namespace AzureDevOps.Exception.Reporter.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View();
        }

        public ActionResult ThrowException()
        {
            throw new WebTestException("Exception thrown from Controller. ", new System.ArgumentNullException("just an inner test exception"));
        }

        public ActionResult About()
        {
            return View();
        }
    }
}