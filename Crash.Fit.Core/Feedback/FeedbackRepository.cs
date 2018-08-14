using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using System.Linq;

namespace Crash.Fit.Feedback
{
    public class FeedbackRepository : RepositoryBase, IFeedbackRepository
    {
        public FeedbackRepository(string connectionString) : base(connectionString)
        {
        }
        public IEnumerable<FeedbackSummary> GetFeedback(string type)
        {
            var sql = @"SELECT Feedback.*, (SELECT COUNT(*) FROM FeedbackVote WHERE FeedbackId=Feedback.Id) AS Score, (SELECT COUNT(*) FROM FeedbackComment WHERE FeedbackId=Feedback.Id) AS CommentCount
FROM Feedback 
WHERE Feedback.Type=@type";
            using (var conn = CreateConnection())
            {
                return conn.Query<FeedbackSummary>(sql, new { type });
            }
        }

        public bool CreateFeedback(Feedback feedback)
        {
            feedback.Id = Guid.NewGuid();

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(@"INSERT INTO Feedback(Id,Type,UserId,Title,Description,Created) VALUES(@Id,@Type,@UserId,@Title,@Description,@Created)", feedback, tran);
                    conn.Execute(@"INSERT INTO FeedbackVote(FeedbackId,UserId,Time) VALUES (@FeedbackId,@UserId,@Time)", new { FeedbackId=feedback.Id,UserId=feedback.UserId,Time = feedback.Created }, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    feedback.Id = Guid.Empty;
                    throw;
                }
            }
        }

        public bool DeleteFeedback(Feedback feedback)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Feedback SET Deleted=@Deleted WHERE Id=@Id", new { Id = feedback.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }



        public FeedbackDetails GetFeedback(Guid id)
        {
            var sql = @"
SELECT Feedback.*, (SELECT COUNT(*) FROM FeedbackVote WHERE FeedbackId=@Id) AS Score
FROM Feedback 
WHERE Id=@id;
SELECT * FROM FeedbackComment WHERE FeedbackId=@Id";
            using (var conn = CreateConnection())
            using(var multi = conn.QueryMultiple(sql, new { id }))
            {
                var feedback = multi.ReadSingle<FeedbackDetails>();
                if(feedback != null)
                {
                    feedback.Comments = multi.Read<FeedbackComment>().ToArray();
                }
                return feedback;
            }
        }

        

        public IEnumerable<Guid> GetVotes(Guid userId)
        {
            var sql = @"SELECT FeedbackId FROM FeedbackVote WHERE UserId=@userId";
            using (var conn = CreateConnection())
            {
                return conn.Query<Guid>(sql, new { userId });
            }
        }

        public bool UpdateFeedback(Feedback feedback)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Feedback SET Title=@Title,Content=@Content WHERE Id=@Id", new { Id = feedback.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public bool UserHasVoted(Guid feedbackId, Guid userId)
        {
            var sql = @"SELECT FeedbackId FROM FeedbackVote WHERE UserId=@userId AND FeedbackId=@feedbackId";
            using (var conn = CreateConnection())
            {
                return conn.QuerySingleOrDefault<Guid?>(sql, new { userId, feedbackId }) != null;
            }
        }

        public bool VoteFeedback(Guid feedbackId, Guid userId)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO FeedbackVote(FeedbackId,UserId,Time) VALUES(@feedbackId,@userId,@Time)", new { feedbackId, userId, Time = DateTimeOffset.Now }, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}
