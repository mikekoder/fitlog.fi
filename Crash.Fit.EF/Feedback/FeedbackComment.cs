using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Feedback
{
    public partial class FeedbackComment
    {
        public Guid Id { get; set; }
        public Guid FeedbackId { get; set; }
        public Guid? UserId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }

        public Feedback Feedback { get; set; }
        public Profile User { get; set; }
    }
}
