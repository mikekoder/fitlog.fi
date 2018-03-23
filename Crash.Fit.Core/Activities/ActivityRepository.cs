using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dapper;
using System.Linq;

namespace Crash.Fit.Activities
{
    public class ActivityRepository : RepositoryBase, IActivityRepository
    {
        public ActivityRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory, connectionString)
        {
        }

        public IEnumerable<Activity> GetActivities()
        {
            var sql = @"SELECT * FROM Activity WHERE Deleted IS NULL";
            using (var conn = CreateConnection())
            {
                return conn.Query<Activity>(sql).ToList();
            }
        }

        public Activity GetActivity(Guid id)
        {
            var sql = @"SELECT * FROM Activity WHERE Id=@id";
            using (var conn = CreateConnection())
            {
                return conn.QuerySingle<Activity>(sql, new { id });
            }
        }

        public void CreateActivity(Activity activity)
        {
            activity.Id = Guid.NewGuid();
            activity.Created = DateTimeOffset.Now;
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Activity(Id, Name,EnergyExpenditure, Created) VALUES(@Id, @Name,@EnergyExpenditure, @Created)", activity, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    activity.Id = Guid.Empty;
                    throw;
                }
            }
        }

        public void UpdateActivity(Activity activity)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Activity SET Name=@Name,EnergyExpenditure=@EnergyExpenditure WHERE Id=@Id", activity, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void DeleteActivity(Activity activity)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Activity SET Deleted=@Deleted WHERE Id=@Id", new { activity.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<EnergyExpenditure> GetEnergyExpenditures(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            var sql = @"SELECT E.*, A.Name AS ActivityName2, W.Duration AS Duration2 FROM EnergyExpenditure E
LEFT JOIN Activity A ON A.Id = E.ActivityId 
LEFT JOIN Workout W ON W.Id = E.WorkoutId
WHERE E.UserId=@userId AND E.Time >= @start AND E.Time <= @end AND E.Deleted IS NULL";
            using (var conn = CreateConnection())
            {
                var expenditures = conn.Query<EnergyExpenditureRaw>(sql, new { userId, start, end }).ToList();
                foreach (var expenditure in expenditures)
                {
                    if (expenditure.ActivityId.HasValue)
                    {
                        expenditure.ActivityName = expenditure.ActivityName2;
                    }
                    if (expenditure.WorkoutId.HasValue)
                    {
                        expenditure.Duration = expenditure.Duration2;
                    }
                }
                return expenditures;
            }
        }

        public void CreateEnergyExpenditure(EnergyExpenditure expenditure)
        {
            expenditure.Id = Guid.NewGuid();
            expenditure.Created = DateTimeOffset.Now;
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO EnergyExpenditure(Id,UserId,Time,ActivityId,Duration,ActivityName,EnergyKcal,WorkoutId,Created) VALUES(@Id,@UserId,@Time,@ActivityId,@Duration,@ActivityName,@EnergyKcal,@WorkoutId,@Created)", expenditure, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    expenditure.Id = Guid.Empty;
                    throw;
                }
            }
        }

        public EnergyExpenditure GetEnergyExpenditure(Guid id)
        {
            using (var conn = CreateConnection())
            {
                return conn.QuerySingleOrDefault<EnergyExpenditure>("SELECT * FROM EnergyExpenditure WHERE Id=@id", new { id });
            }
        }
        public EnergyExpenditure GetEnergyExpenditureForWorkout(Guid workoutId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QuerySingleOrDefault<EnergyExpenditure>("SELECT * FROM EnergyExpenditure WHERE WorkoutId=@workoutId", new { workoutId });
            }
        }
        public void UpdateEnergyExpenditure(EnergyExpenditure expenditure)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE EnergyExpenditure SET Time=@Time,ActivityId=@ActivityId,Duration=@Duration,ActivityName=@ActivityName,EnergyKcal=@EnergyKcal WHERE Id=@Id", expenditure, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public void DeleteEnergyExpenditure(EnergyExpenditure expenditure)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE EnergyExpenditure SET Deleted=@Deleted WHERE Id=@Id", new { expenditure.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<ActivityPreset> GetActivityPresets(Guid userId)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ActivityPreset>("SELECT * FROM ActivityPreset WHERE UserId=@userId AND Deleted IS NULL", new { userId }).ToList();
            }
        }

        public void SaveActivityPresets(IEnumerable<ActivityPreset> presets)
        {
            foreach (var preset in presets)
            {
                if (preset.Id == Guid.Empty)
                {
                    preset.Id = Guid.NewGuid();
                }
            }

            var sql = @"MERGE INTO ActivityPreset
USING(select @Id AS Id) AS Source
ON(ActivityPreset.Id = Source.Id)
WHEN MATCHED THEN
    UPDATE SET Name=@Name,Sleep=@Sleep,Inactivity=@Inactivity,LightActivity=@LightActivity,ModerateActivity=@ModerateActivity,HeavyActivity=@HeavyActivity,Factor=@Factor,Monday=@Monday,Tuesday=@Tuesday,Wednesday=@Wednesday,Thursday=@Thursday,Friday=@Friday,Saturday=@Saturday,Sunday=@Sunday
WHEN NOT MATCHED THEN
    INSERT(Id,UserId,Name,Sleep,Inactivity,LightActivity,ModerateActivity,HeavyActivity,Factor,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,Created) 
VALUES(@Id,@UserId,@Name,@Sleep,@Inactivity,@LightActivity,@ModerateActivity,@HeavyActivity,@Factor,@Monday,@Tuesday,@Wednesday,@Thursday,@Friday,@Saturday,@Sunday,GETDATE());";

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute(@"UPDATE ActivityPreset SET Deleted=@Deleted WHERE UserId IN @userIds AND Id NOT IN @ids", new { Deleted = DateTimeOffset.Now, userIds = presets.Select(d => d.UserId), ids = presets.Select(d => d.Id) }, tran);
                    conn.Execute(sql, presets, tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void SetActivityPresetForDay(Guid userId, DateTimeOffset date, Guid activityPresetId)
        {

            var sql = @"MERGE INTO Day
USING(SELECT @userId AS UserId, @date as Date) AS Source
ON(Day.UserId = Source.UserId AND Day.Date = Source.Date)
WHEN MATCHED THEN
    UPDATE SET ActivityPresetId=@activityPresetId
WHEN NOT MATCHED THEN
    INSERT(UserId,Date,ActivityPresetId) 
VALUES(@userId,@date,@activityPresetId);";

            using (var conn = CreateConnection())
            {
                try
                {
                    conn.Execute(sql, new { userId, date, activityPresetId });
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
        public Dictionary<DateTimeOffset, Guid> GetActivityPresetsForDays(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            using (var conn = CreateConnection())
            {
                try
                {
                    return conn.Query("SELECT Date,ActivityPresetId FROM Day WHERE UserId=@userId AND Date >= @start AND Date <= @end", new { userId, start, end })
                        .ToDictionary(x => (DateTimeOffset)x.Date, x => (Guid)x.ActivityPresetId);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        class EnergyExpenditureRaw : EnergyExpenditure
        {
            public string ActivityName2 { get; set; }
            public TimeSpan? Duration2 { get; set; }
        }
    }
}
