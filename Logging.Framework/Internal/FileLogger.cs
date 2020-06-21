using Logging.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Framework.Internal
{
    public class FileLogger : LoggerProcessorBase
    {
        private readonly CancellationTokenSource _cancellationToken;
        private readonly List<LogData> _cuurentBatch = new List<LogData>();

        public FileLogger(LoggerSettings loggerSettings): base(loggerSettings, new BlockingCollection<LogData>(new ConcurrentQueue<LogData>()))
        {
            _cancellationToken = new CancellationTokenSource();
        }
        //public override async Task ProcessLogQueue()
        //{
        //    foreach (var item in MessageQueue.GetConsumingEnumerable())
        //    {
        //        await ProcessLogs(item);
        //    }
        //}
        protected override async Task ProcessLogQueue()
        {
            while(!_cancellationToken.IsCancellationRequested)
            {
                var limit = Settings.BatchSize <= 0 ? int.MaxValue : Settings.BatchSize;

                while (limit > 0 && MessageQueue.TryTake(out var data))
                {
                    _cuurentBatch.Add(data);
                    limit--;
                }

                if (_cuurentBatch.Count >0 )
                {
                    try
                    {
                        await WriteLog(_cuurentBatch, _cancellationToken.Token);
                    }
                    catch
                    {
                       //
                    }
                    _cuurentBatch.Clear();
                }
                await IntervalAsync(Settings.FlushPeriod, _cancellationToken.Token);
            }
        }

        private async Task WriteLog(IEnumerable<LogData> cuurentBatch, CancellationToken token)
        {
            Directory.CreateDirectory(Settings.LogDirectory);
            foreach (var item in cuurentBatch.GroupBy(GetGrouping))
            {
                var fullName = GetFullName(item.Key);
                var fileInfo = new FileInfo(fullName);
                if (Settings.FileSizeLimit > 0 && fileInfo.Exists && fileInfo.Length > Settings.FileSizeLimit)
                {
                    return;
                }
                using (var streamWriter = File.AppendText(fullName))
                {
                    foreach (var d in item)
                    {
                        await streamWriter.WriteAsync(LogString(d));
                    }
                }
            }
            CheckRetainPolicy();
        }
        private (int Year, int Month, int Day) GetGrouping(LogData logData) => (logData.TimeStamp.Year, logData.TimeStamp.Month, logData.TimeStamp.Day);
        private string GetFullName((int Year, int Month, int Day) key) => Path.Combine(Settings.LogDirectory, $"{Settings.FileName}{key.Year:0000}{key.Month:00}{key.Day:00}.log");
       private string LogString(LogData d) => $"{d.TimeStamp} LogId={Guid.NewGuid().ToString()} LogLevel={d.Level} ApplicationName={d.ApplicationName} EventId={d.EventId} Logger={d.Logger} HostAddress={d.HostAddress}" +
                $" RemoteHostAddress={d.RemoteHostAddress} Browser={d.Browser} Message={d.Message} Exception={d.Exception}{Environment.NewLine}{Environment.NewLine}";
      
        protected void CheckRetainPolicy()
        {
            if (Settings.RetainFileCountLimit <=0 )
            {
                return;
            }
            var files = new DirectoryInfo(Settings.LogDirectory)
                .GetFiles(Settings.FileName + "*")
                .OrderByDescending(n => n.Name)
                .Skip(Settings.RetainFileCountLimit);
            foreach (var item in files)
            {
                item.Delete();
            }
        }
        private Task IntervalAsync(TimeSpan flushPeriod, CancellationToken token)
        {
            return Task.Delay(flushPeriod, token);
        }

        public override Task WriteLog(LogData logData)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            _cancellationToken.Cancel();
            base.Dispose();
        }
    }
}
