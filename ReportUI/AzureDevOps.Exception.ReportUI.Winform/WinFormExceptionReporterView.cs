using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Forms;
using AzureDevOps.Exception.ReportUI.WinForm;
using AzureDevOps.Exception.Reporter;

namespace AzureDevOps.Exception.Report.WinForm
{
    [Export(typeof(IExceptionReportView))]
    public class WinFormExceptionReporterView : IExceptionReportView
    {

        public void ShowException(string errorText, ReportException post, ReportException cancel)
        {
            var window = new ReportFormUI();
            window.errorTxtBox.Text = errorText;
            window.PostBtn.Click += (sender, args) => post(window.Description.Text);
            window.cancelBtn.Click += (sender, args) => cancel(window.Description.Text);

            window.TopLevel = true;
            window.ShowDialog();
        }

        public void ShowDeliveryFailure(string message)
        {
            MessageBox.Show(message, "Delivery failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowTerminateDialog()
        {
            var termForm = new TerminatingForm();
            termForm.Show();
            termForm.Refresh();
            Thread.Sleep(5000);
            termForm.Close();
        }
    }
}