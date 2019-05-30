using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Settings
{
    public class SettingsRepository : RepositoryBase, ISettingsRepository
    {
        public SettingsRepository(string connectionString) : base(connectionString)
        {

        }
        public object GetSettings(Guid userId, string key)
        {
            using (var conn = CreateConnection())
            {
                var json = conn.QuerySingleOrDefault<string>("SELECT Data from UserSettings WHERE UserId=@userId AND [Key]=@key",new { userId, key });
                return SettingsUtils.Deserialize(key, json);
            }
        }

        public void UpdateSettings(Guid userId, string key, object data)
        {
            using (var conn = CreateConnection())
            {
                var json = SettingsUtils.Serialize(key, data);
                conn.Execute(@"MERGE INTO UserSettings
USING(select @userId AS UserId, @key as [Key]) AS Source
ON(UserSettings.UserId = Source.UserId AND UserSettings.[Key] = Source.[Key])
WHEN MATCHED THEN
    UPDATE SET Data = @data, Updated=@time
WHEN NOT MATCHED THEN
    INSERT(UserId, [Key], Data, Created, Updated) VALUES(@userId, @key, @data, @time, @time); ", new { userId, key, data = json, time = DateTimeOffset.UtcNow });
            }
        }
    }
}
