using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Framework.Extensions
{
    public  static class GenericMethods
    {
        internal static GDataReader<T> GetDataReader<T>(this IEnumerable<T> ts)
        {
            return new GDataReader<T>(ts);
        }
    }
}
