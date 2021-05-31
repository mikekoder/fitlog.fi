using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Feedback
{
    public interface IFeedbackRepository
    {
        IEnumerable<FeedbackSummary> GetFeedback(string type);
        FeedbackDetails GetFeedback(Guid id);
        bool CreateFeedback(Feedback feedback);
        bool UpdateFeedback(Feedback feedback);
        bool DeleteFeedback(Feedback feedback);
        bool VoteFeedback(Guid feedbackId, Guid userId);
        bool UserHasVoted(Guid feedbackId, Guid userId);
        IEnumerable<Guid> GetVotes(Guid userId);
    }
}
