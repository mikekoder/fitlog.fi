using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Crash.Fit.Profile
{
    public class ProfileRepository : RepositoryBase, IProfileRepository
    {
        public ProfileRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory, connectionString)
        {
        }
        public Profile GetProfile(Guid userId)
        {
            var sql = @"SELECT * FROM Profile Where UserId=@userID";
            using (var conn = CreateConnection())
            {
                return conn.QuerySingleOrDefault<Profile>(sql, new { userId });
            }
        }
        public bool SaveProfile(Profile profile)
        {
            var sql = @"MERGE INTO Profile
USING (select @UserId AS UserId) AS Source
ON (Profile.UserId=Source.UserId)
WHEN MATCHED THEN
	UPDATE SET DoB=@DoB, Gender=@Gender, Height=@Height,Weight=@Weight, Rmr=@Rmr
WHEN NOT MATCHED THEN
	INSERT(UserId,DoB,Gender,Height,Weight,Rmr) VALUES(@UserId,@DoB,@Gender,@Height,@Weight,@Rmr);";

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(sql, profile, tran);

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }
}
