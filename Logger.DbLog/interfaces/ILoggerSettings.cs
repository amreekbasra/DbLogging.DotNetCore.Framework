using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.DbLog.interfaces
{
    public interface ILoggerSettings
    {
        
		IChangeToken ChangeToken { get; }

        /// <summary>
        /// Gets a value indicating whether the logger should include scope information
        /// </summary>
        bool IncludeScopes { get; }

        /// <summary>
        /// Reloads the configuration
        /// </summary>
        /// <returns>New <see cref="ILoggerSettings"/> instance</returns>
        ILoggerSettings Reload();
        /// <summary>
		/// Retrieves the configured minimum log level for the specified category
		/// </summary>	
		bool TryGetSwitch(string category, out LogLevel level);
    }
}
