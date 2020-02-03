using Logging.Framework.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Internal
{
    public class FileLogger : LoggerProcessorBase
    {
        public FileLogger(LoggerSettings loggerSettings): base(loggerSettings, new BlockingCollection<LogData>(new ConcurrentQueue<LogData>()))
        {

        }
        public override Task ProcessLogQueue()
        {
            throw new NotImplementedException();
        }

        public override Task WriteLog(LogData logData)
        {
            throw new NotImplementedException();
        }
    }
}
