﻿using Microsoft.Extensions.Configuration;
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
        private int fileCountLimit;
        private string fileName;
        private string logDirectory;

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

            logDirectory = config["LogDirectory"];
           // ConnectionString = config["LoggerConnection"];
            EnableSync = enableSync;
            IncludeScope = includeScope;
            FlushPeriod = TimeSpan.TryParse(config["FileCountLimit"], out var timespan) ? timespan : new TimeSpan(0);

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

