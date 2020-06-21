using Dapper;
using Logging.Framework.Extensions;
using Logging.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Internal
{
    public class DbLoggerProcessor : LoggerProcessorBase
    {
        private readonly List<LogData> currentBatch;
        private readonly int maxQueueSize;
        public const string SqlQuery = "Insert into LogEntry ([Application],[EventId], [Level],[Logger], [Message],[Exception]) VALUES (@Application, @EventId, @Level, @logger, @Message, @Exception)";
        public DbLoggerProcessor(LoggerSettings loggerSettings) : base(loggerSettings, new BlockingCollection<LogData>(new ConcurrentQueue<LogData>()))
        {
            var connString = Settings.ConnectionString;
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new InvalidOperationException("Connection string missing or not supported");
            }
            currentBatch = new List<LogData>();
            maxQueueSize = Settings.DbBatchSize > 0 ? Settings.DbBatchSize : 100;
        }


        protected override async Task ProcessLogQueue()
        {
            foreach (var item in MessageQueue.GetConsumingEnumerable())
            {
                await ProcessLogs(item);
            }
        }

        private async Task ProcessLogs(LogData item)
        {
            currentBatch.Add(item);
            await WriteLog(item);
            if (currentBatch.Count >= maxQueueSize || MessageQueue.IsAddingCompleted)
            {
                try
                {
                   // await WriteBulkLog(currentBatch.ToArray());
                   
                    currentBatch.Clear();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[{nameof(DbLoggerProcessor)}] Log throw {ex}");
                }
            }
        }

        public override async Task WriteLog(LogData logData)
        {
            try
            {
                using (var conn = new SqlConnection(Settings.ConnectionString))
                {
                  //  await conn.ExecuteAsync(SqlQuery, logData);
                    await WriteLogtoDB(logData);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[{nameof(DbLoggerProcessor)}] Log throw {ex}");
            }
        }
        private async Task WriteLogtoDB(LogData log)
        {
            var dp = new DynamicParameters();
            dp.Add("@EventId", log.EventId);
            dp.Add("@EventName", log.ApplicationName);
            dp.Add("@LogLevel", log.Level);
            dp.Add("@Category", log.Logger);
            dp.Add("@Scope", log.Browser);
            dp.Add("@Message", log.Message);
            dp.Add("@LogTime", log.TimeStamp);
            dp.Add("@Exception", log.Exception?.ToString());

            using (var con = new SqlConnection(Settings.ConnectionString))
            {
               await con.ExecuteAsync("LogRecordInsert", dp, commandType: CommandType.StoredProcedure);
            };
        }
        protected async Task WriteBulkLog(LogData[] logDatas)
        {
            if (logDatas == null || !logDatas.Any())
            {
                return;
            }

            try
            {
                await Task.Factory.StartNew(async () =>
                {
                    using (var sqlBulk = new SqlBulkCopy(Settings.ConnectionString, SqlBulkCopyOptions.KeepNulls))
                    {
                        sqlBulk.DestinationTableName = "[LogEntry]";
                        sqlBulk.BatchSize = maxQueueSize;
                        sqlBulk.ColumnMappings.Add("Id", "Id");
                        sqlBulk.ColumnMappings.Add("Application", "ApplicationName");
                        sqlBulk.ColumnMappings.Add("EventId", "EventId");
                        sqlBulk.ColumnMappings.Add("Level", "Level");
                        sqlBulk.ColumnMappings.Add("Logger", "Logger");
                        sqlBulk.ColumnMappings.Add("Message", "Message");
                        sqlBulk.ColumnMappings.Add("Exception", "Exception");

                        using (var rdr = logDatas.GetDataReader())
                        {
                            await sqlBulk.WriteToServerAsync(rdr);
                        }
                    }


                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[{nameof(DbLoggerProcessor)}] Log throw {ex}");
            }
        }
    }
}
