using Logger.DbLog.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.DbLog.extensions
{
    public class LoggerSettings : ILoggerSettings
    {
        private readonly IConfiguration _configuration;
        public LoggerSettings(IConfiguration configuration)
        {
            _configuration = configuration;
            ChangeToken = configuration.GetReloadToken();
            string value = string.Empty;
            // Retrieve IncludeScopes setting
            var includeScopes = false;
            value = configuration["IncludeScopes"];
            bool.TryParse(value, out includeScopes);
            IncludeScopes = includeScopes;
        }
        public IChangeToken ChangeToken { get; private set; }

        public bool IncludeScopes { get; }

        public ILoggerSettings Reload()
        {
            ChangeToken = null;
            return new LoggerSettings(_configuration);
        }

        public bool TryGetSwitch(string category, out LogLevel level)
        {
            level = LogLevel.None;

            var switches = _configuration.GetSection("LogLevel");
            if (switches == null)
            {
                return false;
            }

            var value = switches[category];
            return Enum.TryParse(value, out level);
        }
    }
}
