using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.DbLog
{
    public interface ILogWriter
    {
        /// <summary>
        /// Writes a single log record to permanent storage
        /// </summary>
        /// <param name="log"><see cref="LogRecord"/> instance to be written</param>
        void WriteLog(LogRecord log);
    }
}
