using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using System.Windows.Threading;

namespace Inmeta.Exception.ReportUI.WPF 
{

    /// <summary>
    /// Interaction logic for ReportException.xaml
    /// </summary>
    public partial class ReportFormUI 
    {
        public void RegisterExceptionEvents(Action<System.Exception> callback)
        {
        }

        public ReportFormUI()
        {
            InitializeComponent();
        }
    }
}
