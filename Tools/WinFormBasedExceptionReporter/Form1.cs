using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using System.Linq;

namespace ExceptionReporterTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            lblServiceUrl.Text = ConfigurationManager.AppSettings["serviceURL"];
        }

        private void btnThreadFail_Click(object sender, EventArgs e)
        {
            new Thread(() => btnFail_Click(sender, e)).Start();
        }

        private void btnFail_Click(object sender, EventArgs e)
        {
            Action<int> fun = delegate(int x){};
            
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
                if (chkNewWi.Checked)
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
                return new string(Enumerable.Range(0, 260).Select(i => (char) ('A' + ran.Next(30))).ToArray());
            }
        }
    }
}