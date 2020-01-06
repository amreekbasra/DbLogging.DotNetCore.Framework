using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Global.Utils.common
{
    public class Utils
    {        
        public static bool ObjectPropertiesEmpty<T>(T obj)
        {
            bool flag = true;
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                var t = p.GetType();
                var v = p.GetValue(obj);
                var defaultvalue = GetDefaultValue(t);
                if (t == typeof(string))
                {
                    if (!string.IsNullOrWhiteSpace(v.ToString())) { flag = false; break; }
                }
                else if (v != defaultvalue) { flag = false; break; }
            }
            return flag;
        }
        public static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            else
            {
                return null;
            }
        }
    }
}
