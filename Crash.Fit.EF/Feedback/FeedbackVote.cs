using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Feedback
{
    public partial class FeedbackVote
    {
        public Guid Id { get; set; }
        public Guid FeedbackId { get; set; }
        public Guid? UserId { get; set; }
        public DateTimeOffset Time { get; set; }

        public Feedback Feedback { get; set; }
        public Profile User { get; set; }
    }
}
