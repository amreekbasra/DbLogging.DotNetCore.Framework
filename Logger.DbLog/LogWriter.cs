using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace Logger.DbLog
{
    public class LogWriter : ILogWriter
    {
        private readonly string _connectionString;
        public LogWriter(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Writes a single log record to database
        /// </summary>
        /// <param name="log"><see cref="LogRecord"/> instance to be written</param>
        public void WriteLog(LogRecord log)
        {
            var dp = new DynamicParameters();           
           dp.Add("@EventId", log.EventId);
           dp.Add("@EventName", log.EventName);
           dp.Add("@LogLevel", log.LogLevel.ToString());
           dp.Add("@Category", log.Category);
           dp.Add("@Scope", log.Scope);
           dp.Add("@Message", log.Message);
           dp.Add("@LogTime", log.LogTime);
           dp.Add("@Exception", log.Exception?.ToString());

            using ( var con = new SqlConnection(_connectionString))
            {
                con.Execute("LogRecordInsert", dp,commandType: CommandType.StoredProcedure);
            };           
        }

    }
}
