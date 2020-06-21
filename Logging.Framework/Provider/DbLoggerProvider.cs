using Logging.Framework.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Framework.Provider
{
    [ProviderAlias("Database")]
    public class DbLoggerProvider : AsyncLoggerProviderBase
    {
        public DbLoggerProvider(IServiceProvider serviceProvider, LoggerSettings loggerSettings, IAsyncLoggerProcessor loggerProcessor) 
            : base(serviceProvider, loggerSettings, loggerProcessor)
        {
        }
    }
}
