using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global.Utils.common
{
    public class ErrorState
    {
        public List<Errors> ErrorsList;
        private bool state;
        public List<string> MessageList;
        public ErrorState()
        {
            ErrorsList = new List<Errors>();
            MessageList = new List<string>();
        }
        public void AddError( Errors errors)
        {
            ErrorsList.Add(errors);
            if (ErrorsList.Count() > 0) state = true;
        }
        public bool State { get => state; private set => state = value; }
    }

    public class Errors
    {
        public Dictionary<string, string> Error= new Dictionary<string, string>();
        public void AddError(string key, string value)
        {
            if (!Error.Keys.Contains(key))
            {
                Error.Add(key, value);
            }           
        }
    }
}
