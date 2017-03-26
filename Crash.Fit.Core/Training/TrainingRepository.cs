using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Crash.Fit.Training
{
    public class TrainingRepository : RepositoryBase, ITrainingRepository
    {
        public TrainingRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory, connectionString)
        {
        }

        public IEnumerable<MuscleGroup> GetMuscleGroups()
        {
            var sql = @"SELECT * FROM MuscleGroup";
            using (var conn = CreateConnection())
            {
                return conn.Query<MuscleGroup>(sql).ToList();
            }
        }

        public IEnumerable<ExerciseMinimal> SearchExercises(string[] nameTokens, Guid? userId)
        {
            var parameters = new DynamicParameters();
            var sql = "";
            if (nameTokens != null)
            {
                for (int i = 0; i < nameTokens.Length; i++)
                {
                    parameters.Add("p" + i, nameTokens[i]);
                    sql += " AND Name LIKE CONCAT('%',@p" + i + ",'%')";
                }
            }
            if (userId.HasValue)
            {
                sql += " AND (UserId IS NULL OR UserId=@UserId)";
                parameters.Add("UserId", userId.Value);
            }
            else
            {
                sql += " AND UserId IS NULL";
            }
            sql = "SELECT * FROM Food WHERE " + sql.Substring(5) + " ORDER BY Name";
            using (var conn = CreateConnection())
            {
                return conn.Query<ExerciseMinimal>(sql, parameters);
            }
        }
        public IEnumerable<ExerciseMinimal> SearchUserExercises(Guid userId)
        {
            var sql = @"
SELECT * FROM Exercise WHERE UserId=@userId AND Deleted IS NULL;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId }))
            {
                var exercises = multi.Read<ExerciseMinimal>().ToList();
                return exercises;
            }
        }
        public ExerciseDetails GetExercise(Guid id)
        {
            var sql = @"SELECT * FROM Exercise WHERE Id=@id";
            using (var conn = CreateConnection())
            {
                return conn.Query<ExerciseDetails>(sql, new { id }).SingleOrDefault();
            }
        }
        public bool CreateExercise(ExerciseDetails exercise)
        {
            exercise.Id = Guid.NewGuid();
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Exercise(Id, UserId, Name) VALUES(@Id, @UserId, @Name)", exercise, tran);
                    tran.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    exercise.Id = Guid.Empty;
                    return false;
                }
            }
        }
        public bool UpdateExercise(ExerciseDetails exercise)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Exercise SET Name=@Name WHERE Id=@Id", exercise, tran);
                    tran.Commit();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool DeleteExercise(ExerciseMinimal exercise)
        {
            throw new NotImplementedException();
        }
        public bool RestoreExercise(Guid id, out ExerciseDetails exercise)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RoutineMinimal> SearchRoutines(Guid userId)
        {
            throw new NotImplementedException();
        }
        public RoutineDetails GetRoutine(Guid id)
        {
            throw new NotImplementedException();
        }
        public bool CreateRoutine(RoutineDetails routine)
        {
            throw new NotImplementedException();
        }
        public bool UpdateRoutine(RoutineDetails routine)
        {
            throw new NotImplementedException();
        }
        public bool DeleteRoutine(RoutineMinimal routine)
        {
            throw new NotImplementedException();
        }    
        public bool RestoreRoutine(Guid id, out RoutineDetails routine)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WorkoutMinimal> SearchWorkouts(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            throw new NotImplementedException();
        }
        public WorkoutDetails GetWorkout(Guid id)
        {
            throw new NotImplementedException();
        }
        public bool CreateWorkout(WorkoutDetails workout)
        {
            throw new NotImplementedException();
        }
        public bool UpdateWorkout(WorkoutDetails workout)
        {
            throw new NotImplementedException();
        }
        public bool DeleteWorkout(WorkoutMinimal workout)
        {
            throw new NotImplementedException();
        }
        public bool RestoreWorkout(Guid id, out WorkoutDetails workout)
        {
            throw new NotImplementedException();
        }
    }
}
