using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Feedback
{
    public class FeedbackVote : Entity
    {
        public Guid UserId { get; set; }
        public Guid FeedbackId { get; set; }
        public DateTimeOffset Time { get; set; }
    }
}
