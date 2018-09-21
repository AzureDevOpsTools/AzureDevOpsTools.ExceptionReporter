using System;

namespace AzureDevOpsTools.ExceptionService.Common.Stores.TFS
{
    public class ExceptionSpecifics
    {
        public string RefCount { get; set; }
        public string ExceptionReporter { get; set; }
        public string BuildVersion { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionMessageEx { get; set; } = "";
        public string ExceptionType { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string StackChecksum { get; set; }
        public string AssemblyName { get; set; }


        public override string ToString()
        {
            var msg =
                $"Checksum:{StackChecksum}\r\n#Incidents:{RefCount}\r\n#Reported by: {ExceptionReporter}\r\n#ExceptionType:{ExceptionType}\r\n#Assembly:{AssemblyName}\r\n#Class:{Class}\r\n#Method:{Method}\r\n#Source:{Source}\r\n#BuildVersion:{BuildVersion}\r\n#ExceptionMessage:{ExceptionMessage}->->->{ExceptionMessageEx}\r\n#StackTrace:{StackTrace}";
            return msg;
        }


        public static ExceptionSpecifics CreateExceptionSpecifics(string msg)
        {
            var elements = msg.Split('#');
            var es = new ExceptionSpecifics();
            int i = 0;
            foreach (var element in elements)
            {
                var infos = element.Split(':');
                if (infos.Length != 2)
                {
                    i++;
                    continue;
                }

                var info = infos[1].Trim('\n').Trim('\r').Trim();
                switch (i)
                {
                    case 0:
                        es.StackChecksum = info;
                        break;
                    case 1:
                        es.RefCount = info;
                        break;
                    case 2:
                        es.ExceptionReporter = info;
                        break;
                    case 3:
                        es.ExceptionType = info;
                        break;
                    case 4:
                        es.AssemblyName = info;
                        break;
                    case 5:
                        es.Class = info;
                        break;
                    case 6:
                        es.Method = info;
                        break;
                    case 7:
                        es.Source = info;
                        break;
                    case 8:
                        es.BuildVersion = info;
                        break;
                    case 9:
                        es.ExceptionMessage = info;
                        break;
                    case 10:
                        es.StackTrace = info;
                        break;
                }


                i++;
            }

            return es;
        }

        public void IncrementIncidentCount()
        {
            var val = Convert.ToInt32(RefCount) + 1;
            RefCount = val.ToString();
        }


    }
}