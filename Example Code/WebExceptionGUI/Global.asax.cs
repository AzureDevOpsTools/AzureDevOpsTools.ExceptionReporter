using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Osiris.TFSExceptionSystem.UI.WebExceptionGUI
{
    public class Global : System.Web.HttpApplication
    {

        //protected void Application_Start(object sender, EventArgs e)
        //{

        //}

        //protected void Session_Start(object sender, EventArgs e)
        //{

        //}

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{

        //}

        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{

        //}


        protected void Application_Error(object sender, EventArgs e)
        {

            Exception ex = Server.GetLastError();

            if (ex is HttpUnhandledException)
            {
                //Server.Transfer(WebExceptionGUI.Properties.Settings.Default.ExceptionPage);
                Server.Transfer(@"/" + WebExceptionGUI.Properties.Settings.Default.ExceptionPage);
                
                if (Server.GetLastError() == ex)
                    Server.ClearError();
            }
            else
            {
                //TODO: what do we do? ..Nothing..? 
            }
        }
        

        //protected void Session_End(object sender, EventArgs e)
        //{

        //}

        //protected void Application_End(object sender, EventArgs e)
        //{

        //}
    }
}