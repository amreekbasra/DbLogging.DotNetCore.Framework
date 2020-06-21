using Microsoft.Extensions.Primitives;
using System;

namespace Logging.Framework.Models
{
    public class LogData
    {
        public DateTime TimeStamp { get; internal set; }
        public int Id { get; set; }
        public string ApplicationName { get; internal set; }
        public string EventId { get; internal set; }
        public string Level { get; internal set; }
        public string Logger { get; internal set; }        
        public string Message { get; set; }      
        public string Exception { get; internal set; }
        public string HostAddress { get; set; }
        public string RemoteHostAddress { get; set; }
        public string Browser { get; internal set; }
        public string Url { get; internal set; }
       
    }
}
