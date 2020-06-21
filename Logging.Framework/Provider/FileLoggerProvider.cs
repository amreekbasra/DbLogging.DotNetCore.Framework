using Logging.Framework.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Framework.Provider
{
    [ProviderAlias("File")]
    public class FileLoggerProvider : AsyncLoggerProviderBase
    {
        public FileLoggerProvider(IServiceProvider serviceProvider, LoggerSettings loggerSettings, IAsyncLoggerProcessor loggerProcessor) : base(serviceProvider, loggerSettings, loggerProcessor)
        {
        }
    }
}
