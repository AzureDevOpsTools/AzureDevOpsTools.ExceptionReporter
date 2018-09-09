using System.Web.Mvc;
using System.Web.Routing;

namespace AzureDevOps.Exception.Reporter.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            if (routes != null)
            {
                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                //added to 
                routes.IgnoreRoute("{resource}.asmx/{*pathInfo}");

                routes.MapRoute(
                    "Default", // Route name
                    "{controller}/{action}/{id}", // URL with parameters
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                    );
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}