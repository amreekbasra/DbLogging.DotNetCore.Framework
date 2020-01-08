using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Global.Utils.common
{
    public static class Utils
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

        public static void PopulateConfigurations<T>(this T myType, IConfiguration configuration) where T : class
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var item in properties)
            {
                //var type = item.PropertyType;
                try
                {
                    item.SetValue(myType, configuration[item.Name], null);
                }
                catch 
                {
                }
            }
        }
        //public static T Get<T>(this IConfiguration configuration, string key)
        //{
        //   // return configuration.GetSection(key).GetChildren<T>();
        //}
    }
}
