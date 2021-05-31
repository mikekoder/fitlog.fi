using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Settings
{
    public interface ISettingsRepository
    {
        void UpdateSettings(Guid userId, string key, object data);
        object GetSettings(Guid userId, string key);
    }
}
