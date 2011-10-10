using System;
using System.Diagnostics;

namespace Inmeta.Exception.Service.Proxy.Reader
{
    public static class EventLogger
    {
        public static void LogInformation(EventLog log, string information)
        {
            if (log == null) throw new ArgumentNullException("log");
            AddLogEntry(log, information, EventLogEntryType.Information);
        }

        public static void LogWarning(EventLog log, string warning)
        {
            if (log == null) throw new ArgumentNullException("log");
            AddLogEntry(log, warning, EventLogEntryType.Warning);
        }

        public static void LogError(EventLog log, string error)
        {
            if (log == null) throw new ArgumentNullException("log");
            AddLogEntry(log, error, EventLogEntryType.Error);
        }

        public static void LogException(EventLog log, System.Exception exception)
        {
            if (log == null) throw new ArgumentNullException("log");
            LogError(log, exception.ToString());
        }

        #region Internal

        private static void AddLogEntry(EventLog log, string entryText, EventLogEntryType entryType)
        {
            if (log == null) throw new ArgumentNullException("log", "Cannot write to event log; EventLog is null.");
            log.WriteEntry(entryText, entryType);
        }
        #endregion
    }
}
