using Inmeta.Exception.Service.Common;
using Inmeta.Exception.Service.Common.Stores.MSMQ;

namespace QueueTool
{
    public class QueueUtil
    {
        public void CreateMessage()
        {
            using (var queue = new ExceptionQueue())
            {
                queue.SendException(new ExceptionEntity());
            }
        }

        public void Purge()
        {
            using (var queue = new ExceptionQueue())
            {
                queue.Purge();
            }
        }

        public ExceptionEntity RemoveLast()
        {
            using (var queue = new ExceptionQueue())
            {
                return queue.PopException(10);
            }
        }

        public ExceptionEntity ReadWebService()
        {
            var reader = new ExceptionQueueReader.ExceptionQueueReaderClient();
            return reader.GetException();
        }
    }
}
