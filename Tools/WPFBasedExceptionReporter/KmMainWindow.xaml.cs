using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
