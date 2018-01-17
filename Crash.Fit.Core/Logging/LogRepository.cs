using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Logging
{
    public class LogRepository : RepositoryBase, ILogRepository
    {
        public LogRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory,connectionString)
        {
        }
        public void LogException(Guid userId, string requestMethod, string requestPath, string requestBody, Exception ex)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO LogException(UserId,Time,Method,Path,Body,Exception,StackTrace) VALUES(@userId,@Time,@requestMethod,@requestPath,@requestBody,@Message,@StackTrace)", new
                    {
                        userId,
                        Time = DateTimeOffset.Now,
                        requestMethod,
                        requestPath,
                        requestBody,
                        ex.Message,
                        ex.StackTrace
                    }, tran);
                    tran.Commit();
                }
                catch
                {
                }
            }
        }
        public void LogDuration(string message, TimeSpan duration)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO LogDuration(Time,Message,Duration) VALUES(@Time,@message,@duration)", new
                    {
                        Time = DateTimeOffset.Now,
                        message,
                        duration
                    }, tran);
                    tran.Commit();
                }
                catch
                {
                }
            }
        }
        /*
        public void LogAction(Guid userId, string action, string message, Exception ex)
        {
            ;
        }

        public void LogAction<T>(Guid userId, string action, string entityId, string message, Exception ex) where T : Entity
        {
            ;
        }
        */

    }
}
