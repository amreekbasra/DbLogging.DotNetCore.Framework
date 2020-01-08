using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Framework.Interfaces
{
    public interface ICanWait
    {
        Task WaitToComplete();
    }
}
