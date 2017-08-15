using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Feedback
{
    public class FeedbackDetailsResponse
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }
        public DateTimeOffset Created { get; set; }
        public Comment[] Comments { get; set; }

        public class Comment
        {
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public string Content { get; set; }
            public DateTimeOffset Created { get; set; }
        }
    }
}
