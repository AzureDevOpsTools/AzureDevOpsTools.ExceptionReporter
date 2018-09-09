using System;

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

        private void txtError_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }
    }
}
