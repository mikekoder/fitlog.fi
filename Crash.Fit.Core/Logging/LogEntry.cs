using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Logging
{
    public class LogEntry
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Action { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
