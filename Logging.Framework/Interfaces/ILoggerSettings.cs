using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Framework.Interfaces
{
   public  interface ILoggerSettings
    {
        bool IncludeScopes { get; }
    }
}
