using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Settings
{
    public interface ISettingsRepository
    {
        void UpdateSettings(Guid userId, string key, object data);
        object GetSettings(Guid userId, string key);
    }
}
