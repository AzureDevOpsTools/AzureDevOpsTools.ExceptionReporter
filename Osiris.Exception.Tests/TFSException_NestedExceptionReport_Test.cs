using System;
using System.Text;
using Inmeta.Exception.Reporter;
using Fasterflect;
using NUnit.Framework;

namespace Inmeta.Exception.Tests
{

   
    /// <summary>
    /// Summary description for TFSException_NestedExceptionReport_Test
    /// </summary>
    
    public class TFSException_NestedExceptionReport_Test
    {
      
        [Test]
        public void TFSExceptionReport_GetInstance_No_Version()
        {
            System.Exception sysEx;
            try
            {
                throw new System.Exception("1",
                                           new System.Exception("2",
                                                                new System.Exception(
                                                                    "3",
                                                                    new System.
                                                                        Exception("4"))
                                               )
                    );

            }
            catch (System.Exception e)
            {
                sysEx = e;
            }


            var appName = "TestRun";
            var reporter = "Lars";
            var user = "Sral";

            var ex = new TFSExceptionReport(appName, reporter, user, sysEx, "vserion", "desc");

            Assert.IsTrue(String.Equals(ex.GetPropertyValue("ExceptionEntity").GetPropertyValue("ExceptionMessage"),
                                        sysEx.Message), sysEx.Message + System.Environment.NewLine + " is not Equal to " + System.Environment.NewLine + ex.GetPropertyValue("ExceptionEntity").GetPropertyValue("ExceptionMessage"));


            var truth = ex.GetPropertyValue("ExceptionEntity").GetPropertyValue("StackTrace");

            //code copied from generator
            Assert.IsTrue(String.Equals(truth, GenerateStackTrace(sysEx).ToString()), GenerateStackTrace(sysEx).ToString());
        }

        [Test]
        public void TFSExceptionReport_GetInstance_With_Version()
        {

            var appName = "TestRun";
            var reporter = "Lars";
            var user = "Sral";

            System.Exception sysEx;
            try
            {
                throw new System.Exception("1",
                                             new System.Exception("2",
                                                                  new System.Exception(
                                                                      "3",
                                                                      new System.
                                                                          Exception("4"))
                                                 )
                );

            }
            catch (System.Exception e)
            {
                sysEx = e;
            }

            var exReport = new TFSExceptionReport(appName, reporter, user, sysEx, "1.0", "desc");

            Assert.IsTrue(String.Equals(exReport.GetPropertyValue("ExceptionEntity").GetPropertyValue("ExceptionMessage"), sysEx.Message));

            var truth = exReport.GetPropertyValue("ExceptionEntity").GetPropertyValue("StackTrace");
            //verify StackTrace has been correctly created.


            Assert.IsTrue(String.Equals(truth, GenerateStackTrace(sysEx).ToString()), GenerateStackTrace(sysEx).ToString());
        }

        private static StringBuilder GenerateStackTrace(System.Exception ex)
        {
            System.Exception temp = ex.InnerException;

            StringBuilder stackTrace = new StringBuilder();
            stackTrace.AppendLine(ex.StackTrace);

            while (temp != null)
            {
                stackTrace.AppendLine("Inner StackTrace : ");
                stackTrace.AppendLine(temp.StackTrace);

                temp = temp.InnerException;
            }
            return stackTrace;
        }



        [Test]
        public void TFSExceptionReport_GetInstance_With_StackTrace()
        {
            var sysEx =  ExceptionTestConstants.GenerateStackTrace(1, 4);
            var appName = ExceptionTestConstants.APPLICATION_NAME;
            var reporter = "LArs";
            var user = "Sral";
            var ex = new TFSExceptionReport(appName, reporter, user, sysEx, "1.0", "desc");
            
            var exEnt = ex.GetPropertyValue("ExceptionEntity");
            Assert.IsTrue(String.Equals(exEnt.GetPropertyValue("ExceptionMessage"), sysEx.Message));

            var truth = exEnt.GetPropertyValue("StackTrace");

            //verify StackTrace has been correctly created.
            Assert.IsTrue(String.Equals(truth, GenerateStackTrace(sysEx).ToString()), GenerateStackTrace(sysEx).ToString());
        }
    }
}
