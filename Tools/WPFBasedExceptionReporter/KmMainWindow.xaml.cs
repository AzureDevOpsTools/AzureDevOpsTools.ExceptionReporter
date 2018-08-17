using System;
using System.Configuration;
using System.Threading;
using System.Windows;

namespace WPFBasedExceptionReporterTestApp
{
    /// <summary>
    /// Interaction logic for KmMainWindow.xaml
    /// </summary>
    public partial class KmMainWindow : Window
    {
        public KmMainWindow()
        {
            InitializeComponent();
            laServiceUrlText.Content = ConfigurationManager.AppSettings["serviceURL"];
            cbUseReportUi.IsChecked = bool.Parse(ConfigurationManager.AppSettings["ExceptionReporterUseGUI"]);
        }

        private void btnThrow_Click(object sender, RoutedEventArgs e)
        {
            throw new NullReferenceException(txtExMessage.Text);
        }

        private void btnThrowThreaded_Click(object sender, RoutedEventArgs e)
        {
            string message = txtExMessage.Text;
            new Thread(() => { throw new NullReferenceException(message); }).Start();
        }

    }
}
