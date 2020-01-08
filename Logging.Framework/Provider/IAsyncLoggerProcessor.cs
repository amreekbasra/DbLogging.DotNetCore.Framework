using Logging.Framework.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Logging.Framework.Provider
{
    public interface IAsyncLoggerProcessor:IDisposable
    {
        BlockingCollection<LogData> MessageQueue { get; set; }
        Task EnqueueMessage(LogData logData);
        Task WaitToComplete();
    }

    //public interface IAsyncLoggerProcessor<TLoggerProvider>: IAsyncLoggerProcessor where TLoggerProvider: ILoggerProvider
    //{

    //}
}