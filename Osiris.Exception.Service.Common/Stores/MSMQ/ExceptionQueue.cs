using System;
using System.Messaging;

namespace Inmeta.Exception.Service.Common.Stores.MSMQ
{
    [Obsolete]
    public class ExceptionQueue : IDisposable
    {
        private readonly MessageQueue _messageQueue;

        private const string ExceptionQueueName = "ExceptionReporterQueue";

        public ExceptionQueue()
        {
            string queuePath = GetQueuePath(".", ExceptionQueueName);
            _messageQueue = OpenOrCreateMessageQueue(queuePath);
        }

        public void Purge()
        {
            _messageQueue.Purge();
        }

        public void SendException(ExceptionEntity exception)
        {
            Message newMessage = WriteMessage(exception);
            _messageQueue.Send(newMessage);
        }

        public ExceptionEntity CheckForExceptions(int secondsToWait)
        {
            return ReadMessage(_messageQueue.Peek(new TimeSpan(0, 0, 0, secondsToWait)));
        }

        public ExceptionEntity PopException(int secondsToWait)
        {
            try
            {
                return ReadMessage(_messageQueue.Receive(new TimeSpan(0, 0, 0, secondsToWait)));
            }
            catch (MessageQueueException e)
            {
                //no messages in the queue is not an exception
                if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                {
                    return null;
                }
                throw;
            }
        }

        private Message WriteMessage(ExceptionEntity exception)
        {
            return new Message(exception, Formatter);
        }

        private ExceptionEntity ReadMessage(Message message)
        {
            message.Formatter = Formatter;
            return (ExceptionEntity)message.Body;
        }

        private MessageQueue OpenOrCreateMessageQueue(string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
                CreateMessageQueue(queuePath);

            return OpenMessageQueue(queuePath, true);
        }

        private MessageQueue OpenMessageQueue(string queuePath, bool local)
        {
            var accessMode = local ? QueueAccessMode.Send : QueueAccessMode.Receive;
            return new MessageQueue(queuePath, QueueAccessMode.SendAndReceive);
        }

        private void CreateMessageQueue(string queuePath)
        {
            var queue = MessageQueue.Create(queuePath, false);
            queue.SetPermissions("Administrators", MessageQueueAccessRights.FullControl);
        }


        private string GetQueuePath(string machine, string queueName)
        {
            return string.Format(@"{0}\Private$\{1}", machine, queueName);
        }

        private IMessageFormatter Formatter
        {
            get
            {
                return new BinaryMessageFormatter();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                if (_messageQueue != null)
                {
                    _messageQueue.Dispose();
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
