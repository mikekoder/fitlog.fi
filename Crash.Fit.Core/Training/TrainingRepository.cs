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

        public IEnumerable<Exercise> SearchExercises(string[] nameTokens, Guid? userId)
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
                return conn.Query<Exercise>(sql, parameters);
            }
        }
        public IEnumerable<Exercise> SearchUserExercises(Guid userId)
        {
            var sql = @"
SELECT * FROM Exercise WHERE UserId=@userId AND Deleted IS NULL;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId }))
            {
                var exercises = multi.Read<Exercise>().ToList();
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
        public bool DeleteExercise(Exercise exercise)
        {
            throw new NotImplementedException();
        }
        public bool RestoreExercise(Guid id, out ExerciseDetails exercise)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RoutineSummary> SearchRoutines(Guid userId)
        {
            var sql = @"
SELECT *, (SELECT COUNT(*) FROM RoutineWorkout WHERE RoutineId=Routine.Id) AS WorkoutCount FROM Routine WHERE UserId=@userId AND Deleted IS NULL;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId }))
            {
                var routines = multi.Read<RoutineSummary>().ToList();
                return routines;
            }
        }
        public RoutineDetails GetRoutine(Guid id)
        {
            var sql = $@"SELECT * FROM Routine WHERE Id=@id;
SELECT * FROM RoutineWorkout WHERE RoutineId=@id ORDER By [Index];
SELECT * FROM RoutineExercise WHERE RoutineWorkoutId IN (SELECT Id FROM RoutineWorkout WHERE RoutineId=@id) ORDER By [Index];";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { id }))
            {
                var routine = multi.Read<RoutineDetails>().SingleOrDefault();
                if (routine != null)
                {
                    routine.Workouts = multi.Read<RoutineWorkout>().ToArray();
                    var exercises = multi.Read<RoutineExerciseRaw>().ToList();
                    foreach (var workout in routine.Workouts)
                    {
                        workout.Exercises = exercises.Where(e => e.RoutineWorkoutId == workout.Id).Select(e => new RoutineExercise
                        {
                            ExerciseId = e.ExerciseId,
                            Sets = e.Sets,
                            Reps = e.Reps
                        }).ToArray();
                    }
                }
                return routine;
            }
        }
        public bool CreateRoutine(RoutineDetails routine)
        {
            routine.Id = Guid.NewGuid();
            foreach(var workout in routine.Workouts)
            {
                workout.Id = Guid.NewGuid();
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Routine(Id, UserId, Name) VALUES(@Id, @UserId, @Name)", routine, tran);
                    conn.Execute("INSERT INTO RoutineWorkout(Id,RoutineId,[Index],Name) VALUES(@Id,@RoutineId,@Index,@Name)", routine.Workouts.Select((w, i) => new
                    {
                        w.Id,
                        RoutineId = routine.Id,
                        Index = i,
                        w.Name
                    }), tran);
                    conn.Execute("INSERT INTO RoutineExercise(RoutineWorkoutId,[Index],ExerciseId,Sets,Reps) VALUES(@RoutineWorkoutId,@Index,@ExerciseId,@Sets,@Reps)", routine.Workouts.SelectMany(w => w.Exercises.Select((e,i) => new
                    {
                        RoutineWorkoutId = w.Id,
                        Index = i,
                        e.ExerciseId,
                        e.Sets,
                        e.Reps
                    })), tran);

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    routine.Id = Guid.Empty;
                    foreach (var workout in routine.Workouts)
                    {
                        workout.Id = Guid.Empty;
                    }
                    return false;
                }
            }
        }
        public bool UpdateRoutine(RoutineDetails routine)
        {
            var workoutIds = routine.Workouts.Select(w => w.Id).ToArray();

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM RoutineExercise WHERE RoutineWorkoutId IN @ids", new { ids = routine.Workouts.Where(w => w.Id != Guid.Empty).Select(w => w.Id) }, tran);

                    conn.Execute("UPDATE Routine SET Name=@Name WHERE Id=@Id", routine, tran);
                    for(var i=0;i< routine.Workouts.Length; i++)
                    {
                        var workout = routine.Workouts[i];
                        if (workout.Id == Guid.Empty)
                        {
                            workout.Id = Guid.NewGuid();
                            conn.Execute("INSERT INTO RoutineWorkout(Id,RoutineId,[Index],Name) VALUES(@Id,@RoutineId,@Index,@Name)", new
                            {
                                workout.Id,
                                RoutineId = routine.Id,
                                Index = i,
                                workout.Name
                            }, tran);
                        }
                        else
                        {
                            conn.Execute("UPDATE RoutineWorkout SET [Index]=@Index, Name=@Name WHERE Id=@Id", new
                            {
                                workout.Id,
                                Index = i,
                                workout.Name
                            }, tran);
                        }
                    }
                    
                    conn.Execute("INSERT INTO RoutineExercise(RoutineWorkoutId,[Index],ExerciseId,Sets,Reps) VALUES(@RoutineWorkoutId,@Index,@ExerciseId,@Sets,@Reps)", routine.Workouts.SelectMany(w => w.Exercises.Select((e, i) => new
                    {
                        RoutineWorkoutId = w.Id,
                        Index = i,
                        e.ExerciseId,
                        e.Sets,
                        e.Reps
                    })), tran);

                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    for(var i = 0; i < routine.Workouts.Length; i++)
                    {
                        routine.Workouts[i].Id = workoutIds[i];
                    }
                    return false;
                }
            }
        }
        public bool DeleteRoutine(Routine routine)
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
        public bool DeleteWorkout(Workout workout)
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
        class RoutineExerciseRaw
        {
            public Guid RoutineWorkoutId { get; set; }
            public int Index { get; set; }
            public Guid ExerciseId { get; set; }
            public int Sets { get; set; }
            public int Reps { get; set; }
        }
    }
}
