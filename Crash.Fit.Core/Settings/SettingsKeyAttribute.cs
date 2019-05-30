using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Settings
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
