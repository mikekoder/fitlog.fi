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
            var sql = @"
SELECT * FROM Exercise WHERE Id=@id;
SELECT MuscleGroupId FROM ExerciseTarget WHERE ExerciseId=@id;";
            using (var conn = CreateConnection())
                using(var multi = conn.QueryMultiple(sql,new { id }))
            {
                var exercise = multi.Read<ExerciseDetails>().SingleOrDefault();
                if(exercise != null)
                {
                    exercise.Targets = multi.Read<Guid>().ToArray();
                }
                return exercise;
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
                    if (exercise.Targets != null && exercise.Targets.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId) VALUES(@ExerciseId,@MuscleGroupId)", exercise.Targets.Select(t => new { ExerciseId = exercise.Id, MuscleGroupId = t }), tran);
                    }
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
                    conn.Execute("DELETE FROM ExerciseTarget WHERE ExerciseId=@Id", new { exercise.Id }, tran);
                    conn.Execute("UPDATE Exercise SET Name=@Name WHERE Id=@Id", exercise, tran);
                    if (exercise.Targets != null && exercise.Targets.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId) VALUES(@ExerciseId,@MuscleGroupId)", exercise.Targets.Select(t => new { ExerciseId = exercise.Id, MuscleGroupId = t }), tran);
                    }
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

        public IEnumerable<WorkoutSummary> SearchWorkouts(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            var filter = "UserId=@userId AND Time >= @start AND Time <= @end AND Deleted IS NULL";
            var sql = $@"
SELECT * FROM Workout WHERE {filter};
SELECT S.WorkoutId, T.MuscleGroupId, COUNT(S.ExerciseId) as [Count] FROM WorkoutSet S
JOIN Exercise E ON E.Id = S.ExerciseId
JOIN ExerciseTarget T ON T.ExerciseId=E.Id
WHERE S.WorkoutId IN(SELECT Id FROM Workout WHERE {filter})
GROUP BY S.WorkoutId, T.MuscleGroupId";

            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId, start, end }))
            {
                var workouts = multi.Read<WorkoutSummary>().ToList();
                var targets = multi.Read<WorkoutTargetRaw>().ToList();
                foreach(var workout in workouts)
                {
                    workout.MuscleGroupSets = targets.Where(t => t.WorkoutId == workout.Id).ToDictionary(t => t.MuscleGroupId, t => t.Count);
                }
                return workouts;
            }
        }
        public WorkoutDetails GetWorkout(Guid id)
        {
            var sql = @"
SELECT * FROM Workout WHERE Id=@id;
SELECT * FROM WorkoutSet WHERE WorkoutId=@id ORDER BY [Index];";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { id }))
            {
                var workout = multi.Read<WorkoutDetails>().SingleOrDefault();
                if(workout != null)
                {
                    workout.Sets = multi.Read<WorkoutSet>().ToArray();
                }
                return workout;
            }

        }
        public bool CreateWorkout(WorkoutDetails workout)
        {
            workout.Id = Guid.NewGuid();
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Workout(Id, UserId, Time) VALUES(@Id, @UserId, @Time)", workout, tran);
                    conn.Execute("INSERT INTO WorkoutSet(WorkoutId,[Index],ExerciseId,Reps,Weights) VALUES(@WorkoutId,@Index,@ExerciseId,@Reps,@Weights)", workout.Sets.Select((s,i) => new
                    {
                        WorkoutId=workout.Id,
                        Index = i,
                        s.ExerciseId,
                        s.Reps,
                        s.Weights
                    }), tran);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    workout.Id = Guid.Empty;
                    return false;
                }
            }
        }
        public bool UpdateWorkout(WorkoutDetails workout)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM WorkoutSet WHERE WorkoutId=@Id", new { workout.Id }, tran);

                    conn.Execute("UPDATE Workout SET Time=@Time WHERE Id=@Id", workout, tran);
                    conn.Execute("INSERT INTO WorkoutSet(WorkoutId,[Index],ExerciseId,Reps,Weights) VALUES(@WorkoutId,@Index,@ExerciseId,@Reps,@Weights)", workout.Sets.Select((s, i) => new
                    {
                        WorkoutId = workout.Id,
                        Index = i,
                        s.ExerciseId,
                        s.Reps,
                        s.Weights
                    }), tran);
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        public bool DeleteWorkout(WorkoutMinimal workout)
        {
            throw new NotImplementedException();
        }
        public bool RestoreWorkout(Guid id, out WorkoutDetails workout)
        {
            throw new NotImplementedException();
        }

        class WorkoutTargetRaw
        {
            public Guid WorkoutId { get; set; }
            public Guid MuscleGroupId { get; set; }
            public int Count { get; set; }
        }
    }
}
