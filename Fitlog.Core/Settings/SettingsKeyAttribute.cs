using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Settings
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SettingsKeyAttribute : Attribute
    {
        public string Key { get; private set; }
        public SettingsKeyAttribute(string key)
        {
            this.Key = key;
        }
    }
}
