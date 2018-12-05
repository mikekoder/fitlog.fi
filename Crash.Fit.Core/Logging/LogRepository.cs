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
        public LogRepository(string connectionString) : base(connectionString)
        {
        }
        public void LogException(Guid userId, string requestMethod, string requestPath, string requestBody, Exception ex, string clientVersion)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO LogException(UserId,Time,Method,Path,Body,Exception,StackTrace,ClientVersion) VALUES(@userId,@Time,@requestMethod,@requestPath,@requestBody,@Message,@StackTrace,@clientVersion)", new
                    {
                        userId,
                        Time = DateTimeOffset.Now,
                        requestMethod,
                        requestPath,
                        requestBody,
                        ex.Message,
                        ex.StackTrace,
                        clientVersion
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
        public void LogClientVersion(Guid userId, string clientVersion)
        {
            var sql = @"MERGE INTO LogClientVersion
USING(select @userId AS UserId, @clientVersion AS ClientVersion) AS Source
ON(LogClientVersion.UserId = Source.UserId AND LogClientVersion.ClientVersion = Source.ClientVersion)
WHEN MATCHED THEN
    UPDATE SET Time=@Time
WHEN NOT MATCHED THEN
    INSERT (UserId,ClientVersion,Time) VALUES(@userId,@clientVersion,@Time);";
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, new
                    {
                        Time = DateTimeOffset.Now,
                        userId,
                        clientVersion
                    }, tran);
                    tran.Commit();
                }
                catch
                {
                    ;
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
