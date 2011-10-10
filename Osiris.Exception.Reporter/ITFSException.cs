namespace Osiris.Exception.Reporter
{
    public interface ITFSException
    {
        string ApplicationName { get; }
        string UserName { get; }
        string Reporter { get;  }
        string ExceptionClass { get;  }
        string ExceptionMethod { get;  }
        string ExceptionSource { get;  }
        string ExceptionMessage { get;  }
        string ExceptionTitle { get; }
        string Version { get; }
        string StackTrace { get;  }
        string ExceptionType { get; }
        string ReportingStatus { get;}

        /// <summary>
        /// An eventual exception which occured while reporting the original exception. 
        /// Throwing such an exception normally would probably lead to attempting to report it, 
        /// which will probably also fail.
        /// </summary>
        System.Exception ReportingFailure { get; }


        /// <summary>
        /// Posts the exception.
        /// In case of errors during posting, ReportingStatus will be set to "Error", and ReportingFailure will contain the error that occurred.
        /// </summary>
        /// <param name="description">A description from the user with steps to reproduce the exception, if available.</param>
        void Post(string description);
    }
}
