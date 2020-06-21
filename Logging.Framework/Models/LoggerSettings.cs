using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Framework.Models
{
    public class LoggerSettings
    {
        private readonly IConfiguration configuration;
        private int batchSize;
        private int backGroundQueueSize;
        private TimeSpan flushPeriod;
        private int fileSizeLimit;
        private int dbBatchSize;
        private int fileCountLimit;
        private string fileName;
        private string logDirectory;
      //  private int _fileSizeLimit;

        //private IConfiguration configuration;     

        public string Application { get; set; }
        public bool EnableSync { get; set; }
        public string ConnectionString { get; set; }
        public bool IncludeScope { get; set; }
        public LoggerSettings(IConfiguration config): this(config, null)
        {
           // configuration = config;
        }

        public LoggerSettings(IConfiguration config, IConfiguration  connectionStr)
        {
            configuration = config;
            ConnectionString = connectionStr?["Logging"];
            Application = config["Application"];
            bool.TryParse(config["EnableSync"], out bool enableSync);

            fileName = config["FileName"];
            bool.TryParse(config["IncludeScope"], out var includeScope);
           // bool.TryParse(config["EnableSync"], out var enableAsync);
            int.TryParse(config["BackGroundQueueSize"], out backGroundQueueSize);
            int.TryParse(config["BatchSize"], out batchSize);
            int.TryParse(config["FileSizeLimit"], out fileSizeLimit);
            int.TryParse(config["FileCountLimit"], out fileCountLimit);
            TimeSpan.TryParse(config["FlushPeriod"], out var flushPeriod);
            logDirectory = config["LogDirectory"];
           // ConnectionString = config["LoggerConnection"];
            EnableSync = enableSync;
            IncludeScope = includeScope;
           // FlushPeriod = TimeSpan.TryParse(config["RetainedFileCountLimit"], out var timespan) ? timespan : new TimeSpan(0);
            BackGroundQueueSize = backGroundQueueSize;
            BatchSize = batchSize <= 0 ? 32: batchSize;
            FileSizeLimit = fileSizeLimit>0 ? FileSizeLimit * 1024 *1024: 100*1024*1024;
            RetainFileCountLimit = fileCountLimit <=0 ? 30: fileCountLimit;
            LogDirectory = string.IsNullOrWhiteSpace(logDirectory)? "logs": logDirectory;
            FileName = string.IsNullOrWhiteSpace(fileName)? $"{Application}": fileName;
            FlushPeriod = flushPeriod == TimeSpan.Zero? TimeSpan.FromSeconds(1): flushPeriod;
        }

        public LoggerSettings Settings(Action<LoggerSettings> config)
        {
            config.Invoke(null);
            return null;
        }
        public bool TryGetSwitch(string name, out LogLevel logLevel)
        {
            var switches = configuration.GetSection("LogLevel");
            if (switches == null)
            {
                logLevel = LogLevel.None;
                return false;
            }
            var value = switches[name];

            if (string.IsNullOrEmpty(value))
            {
                logLevel = LogLevel.None;
                return false;
            }
            if (Enum.TryParse(value, true, out logLevel))
            {
                return true;
            }
            var message = $"Configuration Value: {value} for Category: {name} is not supported";
            throw new InvalidOperationException(message);
        }
        public int BatchSize { get => batchSize; set => batchSize = value>=0? value : 32; }
        public int BackGroundQueueSize
        {
            get => backGroundQueueSize;
            set { if (value>0) backGroundQueueSize = value; }
        }

        public TimeSpan FlushPeriod
        {
            get => flushPeriod;
            set => flushPeriod = value > TimeSpan.Zero? value: TimeSpan.FromSeconds(1);
        }
        public int FileSizeLimit { get => fileSizeLimit; set => fileSizeLimit = value > 0 ? value : 10 * 1024 * 1024; }
        public int DbBatchSize { get => dbBatchSize; set => dbBatchSize = value >= 0 ? value :100; }
        public int RetainFileCountLimit { get => fileCountLimit; set => fileCountLimit = value > 0 ? value : 60; }
        public string FileName { get => fileName; set => fileName = string.IsNullOrWhiteSpace(value) ? $"{Application}_" : value; }
        public string LogDirectory { get => logDirectory; set => logDirectory = string.IsNullOrWhiteSpace(value) ? "logs" : value; }
        public Func<string, LogLevel, bool> Filter { get; set; }
    }
    
}

