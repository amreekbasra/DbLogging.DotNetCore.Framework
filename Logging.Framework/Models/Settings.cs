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
        //private IConfiguration configuration;
        private object p;

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
            EnableSync = enableSync;
        }

        public LoggerSettings Settings(Action<LoggerSettings> config)
        {
            config.Invoke(null);
            return null;
        }
        public int BatchSize { get => batchSize; set => batchSize = value>=0? value : 10; }
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
        public Func<string, LogLevel, bool> Filter { get; set; }
    }
    }
}
