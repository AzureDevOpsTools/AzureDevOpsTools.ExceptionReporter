using AzureDevOps.Exception.Service.Common.Stores.TFS;

namespace AzureDevOps.Exception.Service.Common
{
    public class TestException : AccessToVsts, IApplicationInfo
    {
        public void SendException()
        {
            var ex = new ExceptionEntity
            {
                ApplicationName = "Test",
                Comment = "Test exception from web",
                StackTrace = "Nothing here",
                TheClass = nameof(TestException),
                TheMethod = nameof(SendException),
                TheSource = "TestException.cs",
                Username = "Tester",
                ExceptionTitle = "Test exception from Web",
                Reporter = "Whoever",
                Version = "1.0.0",
                ExceptionMessage = "Ex msg",
                ExceptionType = "SomeExceptionType",


            };
            var json = CreateNewException(ex, this);
            SendException(json);
        }

        public string ApplicationName => "Whatever";

        public string TfsServer => "Don't care";

        public string Collection => "";

        public string TeamProject => "Nemo Test but is not used";

        public string Area =>"NotSure";

        public string AssignedTo => "Some One";
    }
}
