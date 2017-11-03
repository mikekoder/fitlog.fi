using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Logging
{
    public partial class LogException
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
    }
}
