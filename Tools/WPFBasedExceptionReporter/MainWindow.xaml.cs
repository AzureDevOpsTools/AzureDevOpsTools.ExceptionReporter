using System;
using System.Linq;
using System.Windows;
using System.Threading;
using Inmeta.Exception.Reporter;
using System.Windows.Threading;
using Inmeta.Exception.Service.Common;

namespace WPFBasedExceptionReporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            wsURL_Lbl.Content =  App.ExceptionRegistrator.ServiceSettings.ServiceUrl;
            button1.Click += btnFail_Click;
        }

        private void btnFail_Click(object sender, EventArgs e)
        {
            Action<int> fun = delegate(int x) { };

            fun = (x) =>
            {
                try
                {
                    if (x == 0)
                        throw new NullReferenceException(GENERATE_MESSAGE_GT_260);
                    else
                        fun(x - 1);
                }
                catch (Exception ex)
                {
                    throw new AbandonedMutexException(GENERATE_MESSAGE_GT_260, ex);
                }
            };


            /*Press Continue(F5) if the debugger breaks here. 
             *The exception will then be passed to the global exception handler.*/
            try
            {
                fun(1);
            }
            catch (Exception ex)
            {
                if (uniqueWIT)
                {
                    //create a new exception very similar to the original one, but with some random text added to the stack-trace.

                    try
                    {
                        //Throwing an exception fills some special fields in the exception that can't be set any other way. So i do it! 
                        throw new RandomizedCopyException(ex);
                    }
                    catch (Exception newException)
                    {

                        throw new Exception("", newException);
                    }
                }
                else
                    throw;
            }
        }

        private System.Exception GenerateStackTrace(int version, int depth)
        {
            System.Exception res;
            if (depth == 0)
            {
                try
                {
                    throw new System.Exception(version.ToString());
                }
                catch (System.Exception ex)
                {
                    res = ex;
                }
            }
            else
            {
                try
                {
                    throw new System.Exception(version.ToString(),
                                               GenerateStackTrace(version + 1, depth - 1));
                }
                catch (System.Exception ex)
                {
                    res = ex;
                }
            }

            return res;

        }

        /// <summary>
        /// Generate a message longer than 255
        /// </summary>
        public string GENERATE_MESSAGE_GT_260
        {
            get
            {
                var ran = new Random();
                return new string(Enumerable.Range(0, 260).Select(i => (char)('A' + ran.Next(30))).ToArray());
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            //throw thread from another thread.
            new Thread(() => btnFail_Click(sender, e)).Start();
        }

        private bool uniqueWIT;
        private void chkNewWi_Checked(object sender, RoutedEventArgs e)
        {
            uniqueWIT = chkNewWi.IsChecked.Value;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //spawn 5 threads and throw exceptions.
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                new Thread(() => Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<Exception>(ThrowException), ex)).Start();
                new Thread(() => Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<Exception>(ThrowException), ex)).Start();
                new Thread(() => Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<Exception>(ThrowException), ex)).Start();
                new Thread(() => Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<Exception>(ThrowException), ex)).Start();
                new Thread(() => Application.Current.Dispatcher.Invoke(DispatcherPriority.Send, new Action<Exception>(ThrowException), ex)).Start();
            }
        }

        private void ThrowException(Exception ex)
        {
            if (Thread.CurrentThread == Application.Current.Dispatcher.Thread)
                throw ex;
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            App.ExceptionRegistrator.ReportingUI = ((bool)(checkBox1.IsChecked.HasValue ? checkBox1.IsChecked : false));
        }
    }
}
