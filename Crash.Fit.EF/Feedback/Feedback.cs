using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Feedback
{
    public partial class Feedback
    {
        public Feedback()
        {
            Comments = new HashSet<FeedbackComment>();
            Votes = new HashSet<FeedbackVote>();
        }

        public Guid Id { get; set; }
        public int Type { get; set; }
        public Guid? UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdminComment { get; set; }
        public bool Locked { get; set; }
        public int VoteCount { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public Profile User { get; set; }
        public ICollection<FeedbackComment> Comments { get; set; }
        public ICollection<FeedbackVote> Votes { get; set; }
    }
}
