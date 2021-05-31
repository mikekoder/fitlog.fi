using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Fitlog.Profile
{
    public class ProfileRepository : RepositoryBase, IProfileRepository
    {
        public ProfileRepository(string connectionString) : base(connectionString)
        {
        }
        public Profile GetProfile(Guid userId)
        {
            var sql = @"SELECT * FROM Profile Where UserId=@userID";
            using var conn = CreateConnection();
            return conn.QuerySingleOrDefault<Profile>(sql, new { userId });
        }

        
        public void SaveProfile(Profile profile)
        {
            var sql = @"MERGE INTO Profile
USING (select @UserId AS UserId) AS Source
ON (Profile.UserId=Source.UserId)
WHEN MATCHED THEN
	UPDATE SET DoB=@DoB, Gender=@Gender
WHEN NOT MATCHED THEN
	INSERT(UserId,DoB,Gender) VALUES(@UserId,@DoB,@Gender);";

            using var conn = CreateConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                conn.Execute(sql, profile, tran);

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public string UpdateRefreshToken(Guid userId)
        {
            var token = Guid.NewGuid().ToString() + userId.ToString();
            var sql = @"MERGE INTO Profile
USING (select @userId AS UserId) AS Source
ON (Profile.UserId=Source.UserId)
WHEN MATCHED THEN
	UPDATE SET RefreshToken=@token
WHEN NOT MATCHED THEN
	INSERT(UserId,RefreshToken) VALUES(@userId, @token);";

            using var conn = CreateConnection();
            using var tran = conn.BeginTransaction();
            try
            {
                conn.Execute(sql, new { userId, token }, tran);

                tran.Commit();
                return token;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
        public string GetRefreshToken(Guid userId)
        {
            using var conn = CreateConnection();
            return conn.QuerySingleOrDefault<string>("SELECT RefreshToken FROM Profile WHERE UserId=@userId", new { userId });
        }
        public Guid? GetUserIdByRefreshToken(string refreshToken)
        {
            using var conn = CreateConnection();
            return conn.QuerySingleOrDefault<Guid?>("SELECT UserId FROM Profile WHERE RefreshToken=@refreshToken", new { refreshToken });
        }
    }
}
