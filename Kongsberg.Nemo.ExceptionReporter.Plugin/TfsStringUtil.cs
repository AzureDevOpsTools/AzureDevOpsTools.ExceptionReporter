using System;
using System.Diagnostics.Contracts;

namespace Inmeta.ExceptionReporter.Km
{
    internal static class TFSStringUtil
    {
        /// <summary>
        /// Wash and truncate string to ensure string is a valid TFS string: 
        /// Result will: 
        ///     1. < 256
        ///     2. contains no \r,\n or \t chars.
        ///     3. At least one char length ('_'), if nothing else is present
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GenerateValidTFSStringType(string msg)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            Contract.Ensures(Contract.Result<string>().Length < 256);
            Contract.Ensures(Contract.Result<string>().Length > 0);
            Contract.Ensures(!Contract.Result<string>().Contains("\n"));
            Contract.Ensures(!Contract.Result<string>().Contains("\r"));
            Contract.Ensures(!Contract.Result<string>().Contains("\t"));
            Contract.Ensures(!Contract.Result<string>().Contains(System.Environment.NewLine));

            //empty string is not a valid TFSString type. 
            //Provide at least one '_'. Cannot set to ' ' since space is trimmed away by TFS, resulting in validation error 
            var temp = (msg ?? String.Empty).Length == 0 ? "_" : msg;
            return (temp.Length > 255 ? temp.Substring(0, 255) : temp).Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
        }
    }
}
