using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Logging
{
    public interface ILogRepository
    {
        void LogException(Guid userId, string requestMethod, string requestPath, string requestBody, Exception ex);
        void LogDuration(string message, TimeSpan duration);
        //void LogAction(Guid userId, string action, string message, Exception ex);
        //void LogAction<T>(Guid userId, string action, string entityId, string message, Exception ex) where T : Entity;
    }
}
