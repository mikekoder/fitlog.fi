using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Feedback
{
    public class FeedbackSummaryResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AdminComment { get; set; }
        public bool Locked { get; set; }
        public int Score { get; set; }
        public DateTimeOffset Created { get; set; }
        public int CommentCount { get; set; }
    }
}
