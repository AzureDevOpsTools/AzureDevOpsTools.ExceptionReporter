namespace AzureDevOps.Exception.Reporter
{

    /// <summary>
    /// Delegate used to decorate an exception with a description, the name of the reporter and the application name.
    /// </summary>
    /// <param name="description">A description of the exception and the steps needed to reproduce the error</param>
    public delegate void ReportException(string description);

    /// <summary>
    /// this interface represents an interface which represenst the how the report is displayed.
    /// </summary>
    public interface IExceptionReportView
    {

        /// <summary>
        /// Show the error in the form with the provided text as the error.
        /// </summary>
        /// <param name="errorText">The error text to display.</param>
        /// <param name="post">Report is the delegate to send the report to the TFS reporting service. <see cref="ReportException"/></param>
        /// <param name="cancel">Delegate called when the user clicks cancel</param>
        void ShowException(string errorText, ReportException post, ReportException cancel);

        /// <summary>
        /// This method will be called if a exception occurs during the ingestion into the TFS Exception report service.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        void ShowDeliveryFailure(string message);

        /// <summary>
        /// Show is terminating dialog.
        /// </summary>
        void ShowTerminateDialog();
    }
}