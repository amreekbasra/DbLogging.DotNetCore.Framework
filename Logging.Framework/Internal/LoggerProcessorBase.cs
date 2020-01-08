using Logging.Framework.Models;
using Logging.Framework.Provider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Internal
{
    public abstract class LoggerProcessorBase : IAsyncLoggerProcessor
    {
        public LoggerProcessorBase(LoggerSettings loggerSettings, BlockingCollection<LogData> messageQueue)
        {
            settings = loggerSettings;
            MessageQueue = messageQueue;
            ProcessingTask = Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
        }

        private readonly LoggerSettings settings;
        private readonly Task<Task> ProcessingTask;
        public abstract Task ProcessLogQueue();
        public abstract Task WriteLog(LogData logData);
        public BlockingCollection<LogData> MessageQueue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual async Task EnqueueMessage(LogData logData)
        {
            if (!MessageQueue.IsAddingCompleted)
            {
                MessageQueue.Add(logData);
                return;
            }
            await WriteLog(logData);
        }

        //private Task WriteLog(LogData logData)
        //{
        //    throw new NotImplementedException();
        //}

        public virtual Task WaitToComplete()
        {
            MessageQueue.CompleteAdding();
            return ProcessingTask.Result;
        }
    }
}
