using Logger.DbLog.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.DbLog.extensions
{
    public static class LoggerExtension
    {
        /// <summary>
		/// Adds a custom logging provider
		/// </summary>		
		public static ILoggerFactory AddDbLogger(this ILoggerFactory factory, ILoggerSettings settings, ILogWriter writer)
        {
            factory.AddProvider(new LoggerProvider(settings, writer));
            return factory;
        }

        /// <summary>
        /// Adds a custom logging provider
        public static ILoggerFactory AddDbLogger(this ILoggerFactory factory, IConfiguration loggingConfiguration, ILogWriter writer)
        {
            return factory.AddDbLogger(new LoggerSettings(loggingConfiguration), writer);
        }
    }
}
