using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Crash.Fit.Settings
{
    public static class SettingsUtils
    {
        public static Type GetSettingsType(string key)
        {
            return typeof(SettingsUtils).Assembly.GetTypes()
                .FirstOrDefault(t => t.GetCustomAttribute<SettingsKeyAttribute>()?.Key == key);
        }
        public static object Deserialize(string key, string json)
        {
            var type = GetSettingsType(key);
            if(type == null || json == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(json, type);
        }
        public static string Serialize(string key, object value)
        {
            return JsonConvert.SerializeObject(value);
        }
        public static void Merge(object data, string json)
        {
            JsonConvert.PopulateObject(json, data);
        }
        public static object CreateDefault(string key)
        {
            var type = GetSettingsType(key);
            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type);
        }
    }
}
