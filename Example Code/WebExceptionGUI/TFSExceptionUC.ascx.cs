using System;
using System.Web;
using System.Web.UI;
using Osiris.TFSExceptionSystem;

namespace Osiris.TFSExceptionSystem.UI.WebExceptionGUI
{
    public partial class TFSExceptionUC : UserControl, IExceptionGUI
    {
        #region Fields

        private const string TFSViewStateException = "TFSExceptionViewStateObject";


        #endregion Fields

        #region Properties

        public ITFSException TfsException
        {
            get
            {
                try
                {
                    return (ITFSException)ViewState[TFSViewStateException];
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                // ReSharper restore EmptyGeneralCatchClause
                {

                    //DoNothing()!
                }
                return null;
            }

        }

        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (Properties.Settings.Default.UseTFSExceptionHandling == "false")
                    Response.Redirect(Properties.Settings.Default.ExceptionRedirectUrl);
                DisplayException();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (TfsException != null)
                {
                    //Here's the posting to TFS 
                    PutException(txtDescrption.Text);
                    txtDescrption.Text = "";
                    lblReportingErrorMsg.Text = "The error has been reported and will be investigated";
                    Response.Redirect(Properties.Settings.Default.ExceptionRedirectUrl);
                }
                else
                {
                    txtDescrption.Text = "";
                    lblReportingErrorMsg.Text = "Fant ingen feil å rapportere.";
                }
            }
            catch (Exception ex)
            {
                //Could be endless loops whith exceptions during exception handling
                lblReportingErrorMsg.Text = "Could not send error report: " + ex.Message +"<br />Error Reporting Failure";
            }
        }

    
        protected void btnCancel_Click1(object sender, EventArgs e)
        {
            Cancel();
        }



        #region IExceptionGUI Members

        
        public void DisplayException()
        {
            Exception ex = Server.GetLastError().InnerException;
            txtError.Text = "Message: " + ex.Message + "\n\n" + "StackTrace:\n" + ex.StackTrace;
          
                
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (ex != null)
            {
// ReSharper restore ConditionIsAlwaysTrueOrFalse
                ViewState[TFSViewStateException] = TFSExceptionSystem.TFSException.GetInstance(Properties.Settings.Default.TeamProject, "n/a", HttpContext.Current.User.Identity.Name, ex, Properties.Settings.Default.ApplicationName);
            }
        }

        public void PutException(string description)
        {TfsException.Post(description);
        }

        public void Cancel()
        {
            //Tøm view state og redirect
            ViewState[TFSViewStateException] = null;
            Response.Redirect(Properties.Settings.Default.CancelPage);

        }

        #endregion
    }
}