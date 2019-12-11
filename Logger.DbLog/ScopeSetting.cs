using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Logger.DbLog
{
    public class ScopeSetting
    {
        private static AsyncLocal<ScopeSetting> _value = new AsyncLocal<ScopeSetting>();
        private string _scope;

        public static ScopeSetting Current
        {
            get => _value.Value;
            set => _value.Value = value;
        }


        public ScopeSetting Parent { get; private set; }

        public ScopeSetting(string scope)
        {
            _scope = scope;
        }
        public static IDisposable Push(string scope)
        {
            var temp = Current;
            Current = new ScopeSetting(scope);
            Current.Parent = temp;            

            return new DisposableScope();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
