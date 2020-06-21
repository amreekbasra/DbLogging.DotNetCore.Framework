using Logging.Framework.Models;
using Logging.Framework.Provider;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Logger
{
    public class CustomLogger : ILogger
    {
        public Func<string, LogLevel, bool> Filter => Settings.Filter;

        protected readonly string Name;
        protected IServiceProvider ServiceProvider;
        protected LoggerSettings Settings;
        protected readonly IAsyncLoggerProcessor LoggerProcessor;

        public CustomLogger( string name, IServiceProvider serviceProvider, LoggerSettings loggerSettings, IAsyncLoggerProcessor asyncLoggerProcessor)
        {
            Name = name;
            ServiceProvider = serviceProvider;
            Settings = loggerSettings;
            LoggerProcessor = asyncLoggerProcessor;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return new LoggerScope();
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
                Func<string, LogLevel, bool> funcr = (n, y) => false;
                return  ( n ,m)=> false; 
            }
            foreach (var prefix in GetKeyPrefixes(name))
            {
                if (settings.TryGetSwitch(prefix, out LogLevel level))
                {
                    return (n, m) => m >= level;
                }
            }
            return (n, m) => true;
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

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogAsync(logLevel, eventId, state, exception, formatter).Wait();
        }

        protected virtual async Task LogAsync<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var logData = GetLogData(logLevel, eventId, state, exception, formatter);// new LogData();
            await LoggerProcessor.EnqueueMessage(logData);
        }

        private LogData GetLogData<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
           formatter = formatter?? ((s,t)=> state.ToString());
            var message = formatter(state, exception);
            var httpContextAccessor = (IHttpContextAccessor)ServiceProvider.GetService(typeof(IHttpContextAccessor));//ServiceProvider.GetService<IHttpContextAccessor>();// HttpContext;
            var httpContext = httpContextAccessor?.HttpContext;
            var log = new LogData
            {
                TimeStamp = DateTime.Now,
                Message = message,
                Level = logLevel.ToString(),
                Logger = Name,
                EventId = eventId.ToString(),
                ApplicationName = Settings?.Application,
                Exception = exception?.ToString(),
                Url = httpContext != null ? $"{httpContext.Request.Scheme} :// {httpContext.Request.Host}//{httpContext.Request.Path}" : null
            };
            if (httpContext != null)
            {
                try
                {
                    log.Browser = httpContext.Request.Headers["User-Agent"];
                    log.EventId = log.EventId == "0" ? httpContext.Items["EventId"]?.ToString() ?? "" : log.EventId;
                    log.HostAddress = httpContext.Connection.LocalIpAddress?.MapToIPv4()?.ToString();
                    log.RemoteHostAddress = httpContext.Connection.RemoteIpAddress?.MapToIPv4()?.ToString();
                }
                catch (Exception ex)
                {
                    var error = $"[{nameof(CustomLogger)}] failure parsing User-Agent headers: {ex}";                   
                    Console.Error.WriteLine(error);
                }
                
            }
            return log;
        }
    }

    public class LoggerScope : IDisposable
    {
        public void Dispose()
        {
            
        }
    }
}
