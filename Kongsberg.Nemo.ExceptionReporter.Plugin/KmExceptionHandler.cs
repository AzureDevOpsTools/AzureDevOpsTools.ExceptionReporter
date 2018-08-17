using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using Inmeta.Exception.Reporter;
using Inmeta.Exception.Reporter.TFSExeptionService;
using Kongsberg.Nemo.ExceptionReporter.Plugin.UI;

namespace Inmeta.ExceptionReporter.Km
{
    [Export(typeof(IExceptionHandler))]
    public class KmExceptionHandler : IExceptionHandler
    {
        public KmExceptionHandler()
        {
            // Default properties taken from original KM code
            _properties = new ExceptionHandlerSettings("Kongsberg.Nemo", true, true, "http://exceptions.km.kongsberg.com/web/service.asmx", true, true);
            //Init(properties);
        }

        private ExceptionHandlerSettings _properties;

        private static System.Exception _previousException;
        internal static ReportForm _form;

        private System.Exception TheException { get; set; }

        private static object _syncObject = new object();

        #region IExceptionHandler

        public void OnException(System.Exception e, bool isTerminating)
        {
            var fromSTA = true;

        

            //avoid recursive reporting when IsTerminating is true, since we have registered both Main Form and AppDomain with unhandled exceptions
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                if (_previousException != null && e != null && _previousException.GetHashCode() == e.GetHashCode())
                {
                    KmReportLogger.Instance.LogInfo(new ArgumentException("Trying to report on the same excpetion.", e).ToString());
                    return;
                }

                _previousException = e;
            }

            KmReportLogger.Instance.LogInfo("Received exception  (isTerminating = " + isTerminating + ")");

            //set the excpetion, SAME as NEMO (private static field excpetion)
            TheException = e;

            try
            {
                //use ExceptionHandlerSettings.UseReportGUI to control if UI is to be used or not.
                if (!_properties.UseReportingUi)
                {
                    ReportExceptionWithNoGUI();
                    return;
                }

                // XAML issue with MTA threads.
                if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
                {
                    var threadedForm = new ThreadedReportWindow(e, isTerminating, this);
                    fromSTA = false;
                    return;
                }

                //only report one exception at the time.
                lock (_syncObject)
                {
                    var errorText = Common.Instance.CreateExceptionText(e);

                    //show the error to the user and collect a description of the error from the user.
                    try
                    {
                        //if return value is false -> cancel
                        if (!_form.ShowException(errorText, DoPost))
                        {
                            //cancel report exception to log
                            var report =
                                new KmTFSExceptionReport(
                                    _properties.ApplicationName,
                                    Common.Instance.Reporter,
                                    Common.Instance.Reporter, e,
                                    Common.Instance.Version,
                                    "Exception reported w/o description");

                            KmReportLogger.Instance.LogToFile(report);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        //log report exception 
                        var report =
                            new KmTFSExceptionReport(
                                _properties.ApplicationName,
                                Common.Instance.Reporter,
                                Common.Instance.Reporter, e,
                                Common.Instance.Version,
                                "THIS IS A AUTO GENERATED TEXT: Failed to show exception report.");

                        KmReportLogger.Instance.LogToFile(report);

                        KmReportLogger.Instance.LogExceptionsDuringDelivery(
                            new InvalidOperationException("Failed to show exception report.",
                                                 ex));
                    }
                }
            }
            catch (ThreadAbortException terminate)
            {
                try
                {
                    _form.Window.Close();
                }
                catch
                {
                    //ignore...
                }
                KmReportLogger.Instance.LogInfo(new ThreadStateException("Report form is terminating.", terminate).ToString());
            }
            finally
            {
                //we should inform the user that the application is about to terminate.
                // currently only support for WPF.
                if (_properties.ShowExitWindow && isTerminating && fromSTA && System.Windows.Application.Current != null)
                {
                    try
                    {
                        var terminateWindow = new TerminatingWindow();
                        terminateWindow.Topmost = true;
                        terminateWindow.Show();

                        //sleep for 5000 seconds.
                        Thread.Sleep(5000);

                        terminateWindow.Close();
                    }
                    catch
                    {
                        //for now ignore ... this will happen if thread is MTA...but no more time to code. 
                    }
                }
            }
        }

        public void Init(ExceptionHandlerSettings properites)
        {
            // if emty properties set, use default from constructor
            if (properites != null)
               _properties = properites;

            _form = new ReportForm();
            _form.RegisterExceptionEvents(OnException, _properties.TryContinueAfterException);
            KmReportLogger.Instance.LogExceptionReports = _properties.LogExceptionReports;
        }
        #endregion
       
        private void DoPost(string description)
        {
            try
            {
                //create exception entity
                //Same as NEMO:
                var report = new KmTFSExceptionReport
                    (
                    _properties.ApplicationName,
                    Common.Instance.Reporter,
                    Common.Instance.Reporter, 
                    TheException,
                    Common.Instance.Version,
                    description);

                //log to file
                KmReportLogger.Instance.LogToFile(report);

                //post to service. 
                var result = Post(report);

                KmReportLogger.Instance.LogInfo("Exception posted");
                //if error show to user.
                if (result != null)
                {
                    KmReportLogger.Instance.LogExceptionsDuringDelivery(
                            new FileLoadException("Failed to deliver exception to url = '" + _properties.ServiceUrl + "'", result));
                    try
                    {
                        //failed to deliver exception display for user.
                        _form.ShowDeliveryFailure(result.Message, result);

                    }
                    catch (System.Exception ex)
                    {
                        //failed to show delivery failure... just log 
                        KmReportLogger.Instance.LogExceptionsDuringDelivery(
                            new InvalidOperationException("Failed to show delivery exception", ex));
                    }
                }
            }
            catch (System.Exception ex)
            {
                KmReportLogger.Instance.LogExceptionsDuringDelivery(
                    new FileLoadException("Exception during TFSExceptionREport create or post", ex));
            }
        }

        internal System.Exception Post(KmTFSExceptionReport report)
        {
            try
            {
                // Check for settings overrides
                var serviceUrl = _properties.ServiceUrl.ToLower();

                //if empty ServiceURL do not even try to post to service.
                if (String.IsNullOrEmpty(serviceUrl) || serviceUrl == "none")
                    return null;

                //I KM så bruker man ikke credentials.
                var client = new Service { Url = serviceUrl };

                client.AddNewApplicationException(report.ExceptionEntity);
            }
            catch (System.Exception e)
            {
                KmReportLogger.Instance.LogExceptionsDuringDelivery(e);

                //change in behavior: exception is now returned on failure.
                return e;
            }

            return null;
        }

        internal void ReportExceptionWithNoGUI()
        {
            try
            {
                var report = new KmTFSExceptionReport
                    (
                    _properties.ApplicationName,
                    Common.Instance.Reporter,
                    Common.Instance.Reporter,
                    TheException,
                    Common.Instance.Version,
                    "Exception reported w/o description");

                KmReportLogger.Instance.LogToFile(report);
                Post(report);
                KmReportLogger.Instance.LogInfo("Exception posted with no GUI");
            }
            catch (System.Exception ex)
            {
                KmReportLogger.Instance.LogExceptionsDuringDelivery(new FileLoadException("Failed to deliver exception (no GUI)", ex));
            }
        }
    }
}
