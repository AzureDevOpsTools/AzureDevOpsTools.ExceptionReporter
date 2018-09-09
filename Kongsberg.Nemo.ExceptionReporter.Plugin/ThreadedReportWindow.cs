using System.Threading;
using System.Windows;

namespace AzureDevOps.ExceptionReporter.Km
{
    internal class ThreadedReportWindow
    {
        public Window Parent { get; set; }

        public ThreadedReportWindow(System.Exception e, bool isTerminating, KmExceptionHandler handler)
        {
            try
            {
                KmReportLogger.Instance.LogInfo("Need to spawn own STA thread.");
                var _staReportFormThread = new Thread(() => handler.OnException(e, isTerminating));
                //
                _staReportFormThread.SetApartmentState(ApartmentState.STA);
                _staReportFormThread.Start();
                _staReportFormThread.Join();
            }
            catch (System.Exception failed)
            {
                KmReportLogger.Instance.LogExceptionsDuringDelivery(failed);
            }
        }

        
    }
}