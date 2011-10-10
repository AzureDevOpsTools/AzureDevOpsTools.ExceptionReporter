using System.ComponentModel.Composition;
using System.Web;
using Inmeta.Exception.Reporter;

namespace Inmeta.Exception.ReportUI.Web
{
    [Export(typeof(IExceptionReportView))]
    public class WebExceptionReporterView : IExceptionReportView
    {

        public void ShowException(string errorText, ReportException post, ReportException cancel)
        {
            //not able to show
            //just post exception
            post("Exception reported from web application at url = " + HttpContext.Current.Request.Url.ToString());
        }

        public void ShowDeliveryFailure(string message)
        {
            //not able to show.
        }

        public void ShowTerminateDialog()
        {
            //not able to show.
        }
    }
}