using Logging.Framework.Models;
using Logging.Framework.Provider;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Framework.Logger
{
    public class Logger : ILogger
    {
        public Func<string, LogLevel, bool> Filter => Settings.Filter;

        public string Name { get; }
        public IServiceProvider ServiceProvider { get; }
        public LoggerSettings Settings { get; }
        public IAsyncLoggerProcessor LoggerProcessor { get; }

        public Logger( string name, IServiceProvider serviceProvider, LoggerSettings loggerSettings, IAsyncLoggerProcessor asyncLoggerProcessor)
        {
            Name = name;
            ServiceProvider = serviceProvider;
            Settings = loggerSettings;
            LoggerProcessor = asyncLoggerProcessor;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            if (Settings!= null)
            {
                return GetFilter(Name, Settings).Invoke(Name, logLevel);
            }
            return Filter == null || Filter(Name, logLevel);
        }

        protected virtual Func<string, LogLevel, bool> GetFilter(string name, LoggerSettings settings)
        {
            if (Filter != null)
            {
                return Filter;
            }
            if (settings == null)
            {
              //  var func = (n,1) => false;
              
            }
            foreach (var prefix in GetKeyPrefixes(name))
            {
                if (settings.TryGetSwitch(prefix,out LogLevel l))
                {

                }
            }
        }

        protected virtual IEnumerable<string> GetKeyPrefixes(string name)
        {
            while (!string.IsNullOrEmpty(name))
            {
                yield return name;
                var lastIndexOfDot = name.LastIndexOf('.');
                if (lastIndexOfDot ==-1)
                {
                    yield return "Default";
                    break;
                }
                name = name.Substring(0, lastIndexOfDot);
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }
    }
}
