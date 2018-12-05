using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using System.Data.Common;

namespace Crash.Fit.Measurements
{
    public class MeasurementRepository : RepositoryBase, IMeasurementRepository
    {
        public MeasurementRepository(string connectionString) : base(connectionString)
        {
        }
        public IEnumerable<MeasureSummary> GetMeasures(Guid userId)
        {
            var sql = @"SELECT *
FROM
(
  SELECT Measure.*,Measurement.Time AS LatestTime,Measurement.Value AS LatestValue, ROW_NUMBER() OVER(PARTITION BY Measure.Id ORDER BY Measurement.Time DESC) rownumber
  FROM Measure
  LEFT JOIN Measurement ON Measurement.MeasureId=Measure.Id AND Measurement.UserId=@userId
  WHERE Measure.UserId=@userId OR Measure.UserId IS NULL
) x
WHERE rownumber=1
ORDER BY Name";
            using (var conn = CreateConnection())
            {
                return conn.Query<MeasureSummary>(sql, new { userId });
            }
        }
        public bool CreateMeasure(Measure measure)
        {
            measure.Id = Guid.NewGuid();

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Measure(Id,UserId,Name,Unit) VALUES(@Id,@UserId,@Name,@Unit)", measure, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();                 
                    measure.Id = Guid.Empty;
                    throw;
                }
            }
        }
        public bool CreateMeasurement(Measurement measurement)
        {
            measurement.Id = Guid.NewGuid();

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Measurement(Id,UserId,MeasureId,Time,Value) VALUES(@Id,@UserId,@MeasureId,@Time,@Value)", measurement, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    tran.Rollback();
                    measurement.Id = Guid.Empty;
                    throw;
                }
            }
        }
        public bool UpdateMeasurement(Measurement measurement)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Measurement SET MeasureId=@MeasureId, Time=@Time, Value=@Value WHERE Id=@Id", measurement, tran);
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
        public IEnumerable<Measurement> GetMeasurementHistory(Guid measureId, Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            var sql = @"SELECT * FROM Measurement WHERE MeasureId=@measureId AND UserId=@userId AND Time >= @start AND Time <= @end ORDER BY Time";
            using (var conn = CreateConnection())
            {
                return conn.Query<Measurement>(sql, new { measureId, userId, start, end });
            }
        }

        
    }
}
