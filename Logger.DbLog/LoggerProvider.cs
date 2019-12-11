using Logger.DbLog.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Logger.DbLog
{
   public  class LoggerProvider : ILoggerProvider
    {
       

        private readonly ConcurrentDictionary<string, CustomLogger> _loggers = new ConcurrentDictionary<string, CustomLogger>();

        private readonly ILogWriter _writer;
        private readonly IConfiguration _configuration;
        private ILoggerSettings _settings;

        public LoggerProvider(ILoggerSettings settings, ILogWriter writer)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _settings = settings;
            _writer = writer;
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = new CustomLogger(categoryName, GetFilter(categoryName), _writer, _settings);
            return _loggers.GetOrAdd(categoryName, logger);
        }
        private Func<string, LogLevel, bool> GetFilter(string category)
        {
            if (_settings != null)
            {
                foreach (var prefix in GetKeyPrefixes(category))
                {
                    LogLevel level;

                    if (_settings.TryGetSwitch(prefix, out level))
                    {
                        return (cat, lev) => lev >= level;
                    }
                }
            }

            return (cat, lev) => false;
        }

        private static IEnumerable<string> GetKeyPrefixes(string category)
        {
            while (!string.IsNullOrEmpty(category))
            {
                yield return category;
                var lastIndexOfDot = category.LastIndexOf('.');

                if (lastIndexOfDot == -1)
                {
                    yield return "Default";
                    break;
                }

                category = category.Substring(0, lastIndexOfDot);
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // TODO: dispose managed state (managed objects).
        //            _logger = null;
        //        }

        //        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        //        // TODO: set large fields to null.

        //        disposedValue = true;
        //    }
        //}

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LoggerProvider()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
           // Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
