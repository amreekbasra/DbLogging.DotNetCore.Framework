using Logging.Framework.Models;
using Logging.Framework.Provider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Internal
{
    public abstract class LoggerProcessorBase : IAsyncLoggerProcessor
    {
        private readonly LoggerSettings settings;
        private readonly Task<Task> ProcessingTask;
        public BlockingCollection<LogData> MessageQueue { get ; set; }
        public LoggerProcessorBase(LoggerSettings loggerSettings, BlockingCollection<LogData> messageQueue)
        {
            settings = loggerSettings;
            MessageQueue = messageQueue;
            ProcessingTask = Task.Factory.StartNew(ProcessLogQueue, TaskCreationOptions.LongRunning);
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
        public abstract Task WriteLog(LogData logData);
        public abstract Task ProcessLogQueue();

        public void Dispose()
        {
            WaitToComplete().Wait(1000);
        }
        public virtual Task WaitToComplete()
        {
            MessageQueue.CompleteAdding();
            return ProcessingTask.Result;
        }
        public  virtual void WriteError(string message)
        {
            Debug.WriteLine(message);
            Console.Error.WriteLine(message);
        }
    }
}
