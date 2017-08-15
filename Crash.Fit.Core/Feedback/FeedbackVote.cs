using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Feedback
{
    public class FeedbackVote
    {
        public Guid UserId { get; set; }
        public Guid FeedbackId { get; set; }
        public DateTimeOffset Time { get; set; }
    }
}
