using System;
using System.Text;
using Logger.DbLog.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Logger.DbLog
{
    public  class CustomLogger : ILogger
    {
        private readonly ILogWriter _writer;
        private Func<string, LogLevel, bool> _filter;

        public CustomLogger(string category, Func<string, LogLevel, bool> filter, ILogWriter writer, ILoggerSettings settings)
        {
            _writer = writer;
            _filter = filter;
            Category = category;
            Settings = settings;
        }

        public string Category { get; }
        public ILoggerSettings Settings { get; set; }
        /// <summary>
		/// Gets or sets the delegate used for filtering whether a log record should be discarded
		/// </summary>
		public Func<string, LogLevel, bool> Filter
        {
            get => _filter;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _filter = value;
            }
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return ScopeSetting.Push(state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Filter(Category, logLevel);
        }
        public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter
    )
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            var msg = formatter(state, exception);
            var json = JsonConvert.SerializeObject(new
            {
                logLevel = logLevel,
                eventId = eventId,
                logDateTimeUtc = DateTime.UtcNow,
                details = msg,
                exception = exception
            });
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var log = new LogRecord(eventId.Id, eventId.Name, logLevel, Category, GetScope(), state.ToString(), exception);

            _writer.WriteLog(log);
            Console.WriteLine(json);
        }
        private string GetScope()
        {
            if (!Settings.IncludeScopes)
            {
                return null;
            }

            var current = ScopeSetting.Current;
            var scope = new StringBuilder();

            while (current != null)
            {
                scope.Append(current);

                if (current.Parent != null)
                {
                    scope.AppendLine();
                    scope.Append("=> ");
                }

                current = current.Parent;
            }

            return scope.ToString();
        }

        public class LoggerScope:IDisposable
        {
            public void Dispose()
            {

            }
        }
    }
}