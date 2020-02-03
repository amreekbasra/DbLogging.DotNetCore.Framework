using Logging.Framework.Interfaces;
using Logging.Framework.Logger;
using Logging.Framework.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Provider
{
    public class AsyncLoggerProviderBase : ILoggerProvider, ICanWait
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LoggerSettings _loggerSettings;
        private readonly IAsyncLoggerProcessor _loggerProcessor;

        public AsyncLoggerProviderBase(IServiceProvider serviceProvider, LoggerSettings loggerSettings, IAsyncLoggerProcessor loggerProcessor)
        {
            _serviceProvider = serviceProvider;
            _loggerSettings = loggerSettings;
            _loggerProcessor = loggerProcessor;
        }
        public virtual ILogger CreateLogger(string categoryName)
        {
            return new CustomLogger(categoryName, _serviceProvider, _loggerSettings, _loggerProcessor);
        }

        public virtual void Dispose()
        {
            
        }

        public Task WaitToComplete()
        {
            return _loggerProcessor.WaitToComplete();
        }
    }
}
