using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Feedback
{
    public class FeedbackRequest
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
