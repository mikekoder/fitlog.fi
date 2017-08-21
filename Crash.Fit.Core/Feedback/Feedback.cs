using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Feedback
{
    public class Feedback : Entity
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdminComment { get; set; }
        public bool Locked { get; set; }
    }
    public class FeedbackSummary : Feedback
    {
        public int Score { get; set; }
        public int CommentCount { get; set; }
    }
    public class FeedbackDetails : Feedback
    {
        public int Score { get; set; }
        public FeedbackComment[] Comments { get; set; }
    }
}
