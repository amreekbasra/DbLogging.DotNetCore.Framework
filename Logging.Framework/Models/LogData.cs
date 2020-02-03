using Microsoft.Extensions.Primitives;
using System;

namespace Logging.Framework.Models
{
    public class LogData
    {
        public int LogId { get; set; }
        public int MyProperty { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; internal set; }
        public string Level { get; internal set; }
        public string Logger { get; internal set; }
        public string EventId { get; internal set; }
        public string ApplicationName { get; internal set; }
        public string Exception { get; internal set; }
        public string Url { get; internal set; }
        public string Browser { get; internal set; }
    }
}
