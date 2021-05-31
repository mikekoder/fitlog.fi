using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Fitlog.Settings
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

            return JsonSerializer.Deserialize(json, type);
        }
        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value);
        }
        public static void Merge(object data, string json)
        {
            var type = data.GetType();
            var newData = JsonSerializer.Deserialize(json, type);
            foreach(var prop in type.GetProperties())
            {
                prop.SetValue(data, prop.GetValue(newData));
            }
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
