using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Feedback
{
    public class FeedbackComment : Entity
    {
        public Guid FeedbackId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
