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
        public IEnumerable<ExerciseDetails> SearchUserExercises(Guid userId, DateTimeOffset start1RM)
        {
            var filter = "UserId=@userId AND Deleted IS NULL";
            var sql = $@"
SELECT Exercise.*,(SELECT COUNT(*) FROM WorkoutSet WHERE ExerciseId=Exercise.Id) AS UsageCount, (SELECT MAX (S.OneRepMax) FROM WorkoutSet S JOIN Workout W ON W.Id=S.WorkoutId WHERE S.ExerciseId=Exercise.Id AND W.Time > @time AND W.UserId=@userId) AS OneRepMax FROM Exercise WHERE {filter};
SELECT * FROM ExerciseTarget WHERE ExerciseId IN (SELECT Id FROM Exercise WHERE {filter})";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId, time = start1RM }))
            {
                var exercises = multi.Read<ExerciseDetails>().ToList();
                var targets = multi.Read<ExerciseTargetRaw>().ToList();
                foreach(var exercise in exercises)
                {
                    exercise.Targets = targets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                }
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
            exercise.Created = DateTimeOffset.Now;
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Exercise(Id, UserId, Name,Created) VALUES(@Id, @UserId, @Name,@Created)", exercise, tran);
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
                    throw;
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
                    tran.Rollback();
                    throw;
                }
            }
        }
        public bool DeleteExercise(Exercise exercise)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Exercise SET Deleted=@Deleted WHERE Id=@Id", new { exercise.Id, Deleted = DateTimeOffset.Now }, tran);
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
                        workout.Exercises = exercises.Where(e => e.RoutineWorkoutId == workout.Id).ToArray();
                    }
                }
                return routine;
            }
        }
        public bool CreateRoutine(RoutineDetails routine)
        {
            routine.Id = Guid.NewGuid();
            routine.Created = DateTimeOffset.Now;
            foreach(var workout in routine.Workouts)
            {
                workout.Id = Guid.NewGuid();
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Routine(Id, UserId, Name,Created) VALUES(@Id, @UserId, @Name,@Created)", routine, tran);
                    conn.Execute("INSERT INTO RoutineWorkout(Id,RoutineId,[Index],Name,Frequency) VALUES(@Id,@RoutineId,@Index,@Name,@Frequency)", routine.Workouts.Select((w, i) => new
                    {
                        w.Id,
                        RoutineId = routine.Id,
                        Index = i,
                        w.Name,
                        w.Frequency
                    }), tran);
                    conn.Execute("INSERT INTO RoutineExercise(RoutineWorkoutId,[Index],ExerciseId,Sets,Reps,LoadFrom,LoadTo) VALUES(@RoutineWorkoutId,@Index,@ExerciseId,@Sets,@Reps,@LoadFrom,@LoadTo)", routine.Workouts.SelectMany(w => w.Exercises.Select((e,i) => new
                    {
                        RoutineWorkoutId = w.Id,
                        Index = i,
                        e.ExerciseId,
                        e.Sets,
                        e.Reps,
                        e.LoadFrom,
                        e.LoadTo
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
                    tran.Rollback();
                    throw;
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
                    conn.Execute("DELETE FROM RoutineExercise WHERE RoutineWorkoutId IN (SELECT Id FROM RoutineWorkout WHERE RoutineId=@Id)", new { routine.Id }, tran);               
                    conn.Execute("DELETE FROM RoutineWorkout WHERE Id NOT IN @ids", new { ids = routine.Workouts.Where(w => w.Id != Guid.Empty).Select(w => w.Id) }, tran);

                    for(var i = 0; i< routine.Workouts.Length; i++)
                    {
                        var workout = routine.Workouts[i];
                        if(workout.Id == Guid.Empty)
                        {
                            workout.Id = Guid.NewGuid();
                            conn.Execute("INSERT INTO RoutineWorkout(Id,RoutineId,[Index],Name,Frequency) VALUES(@Id,@RoutineId,@Index,@Name,@Frequency)", new
                            {
                                workout.Id,
                                RoutineId = routine.Id,
                                Index = i,
                                workout.Name,
                                workout.Frequency
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

                    conn.Execute("UPDATE Routine SET Name=@Name WHERE Id=@Id", routine, tran);
                    
                    conn.Execute("INSERT INTO RoutineExercise(RoutineWorkoutId,[Index],ExerciseId,Sets,Reps,LoadFrom,LoadTo) VALUES(@RoutineWorkoutId,@Index,@ExerciseId,@Sets,@Reps,@LoadFrom,@LoadTo)", routine.Workouts.SelectMany(w => w.Exercises.Select((e, i) => new
                    {
                        RoutineWorkoutId = w.Id,
                        Index = i,
                        e.ExerciseId,
                        e.Sets,
                        e.Reps,
                        e.LoadFrom,
                        e.LoadTo
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
                    tran.Rollback();
                    throw;
                }
            }
        }
        public bool DeleteRoutine(Routine routine)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Routine SET Deleted=@Deleted WHERE Id=@Id", new { routine.Id, Deleted = DateTimeOffset.Now }, tran);
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
        public bool RestoreRoutine(Guid id, out RoutineDetails routine)
        {
            throw new NotImplementedException();
        }
        public bool ActivateRoutine(Guid userId, Guid routineId)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Routine SET Active=0 WHERE UserId=@userId", new { userId }, tran);
                    conn.Execute("UPDATE Routine SET Active=1 WHERE Id=@routineId", new { routineId }, tran);
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
    
        public IEnumerable<WorkoutDetails> SearchWorkouts(Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            var filter = "UserId=@userId AND Time >= @start AND Time <= @end AND Deleted IS NULL";
            var sql = $@"
SELECT * FROM Workout WHERE {filter};

SELECT S.WorkoutId, T.MuscleGroupId, COUNT(S.ExerciseId) as [Count] FROM WorkoutSet S
JOIN Exercise E ON E.Id = S.ExerciseId
JOIN ExerciseTarget T ON T.ExerciseId=E.Id
WHERE S.WorkoutId IN(SELECT Id FROM Workout WHERE {filter})
GROUP BY S.WorkoutId, T.MuscleGroupId;

SELECT WS.*, E.Name as ExerciseName FROM WorkoutSet WS
JOIN Exercise E ON E.Id = WS.ExerciseId
WHERE WS.WorkoutId IN (SELECT Id FROM Workout WHERE {filter}) ORDER BY [Index];";

            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId, start, end }))
            {
                var workouts = multi.Read<WorkoutDetails>().ToList();
                var targets = multi.Read<WorkoutTargetRaw>().ToList();
                var sets = multi.Read<WorkoutSetRaw>().ToList();
                foreach(var workout in workouts)
                {
                    workout.MuscleGroupSets = targets.Where(t => t.WorkoutId == workout.Id).ToDictionary(t => t.MuscleGroupId, t => t.Count);
                    workout.Sets = sets.Where(s => s.WorkoutId == workout.Id).ToArray();
                }
                return workouts;
            }
        }
        public WorkoutDetails GetWorkout(Guid id)
        {
            var sql = @"
SELECT * FROM Workout WHERE Id=@id;

SELECT S.WorkoutId, T.MuscleGroupId, COUNT(S.ExerciseId) as [Count] FROM WorkoutSet S
JOIN Exercise E ON E.Id = S.ExerciseId
JOIN ExerciseTarget T ON T.ExerciseId=E.Id
WHERE S.WorkoutId=@id
GROUP BY S.WorkoutId, T.MuscleGroupId;

SELECT WS.*, E.Name as ExerciseName FROM WorkoutSet WS
JOIN Exercise E ON E.Id = WS.ExerciseId
WHERE WorkoutId=@id ORDER BY [Index];";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { id }))
            {
                var workout = multi.Read<WorkoutDetails>().SingleOrDefault();
                if(workout != null)
                {
                    workout.MuscleGroupSets = multi.Read<WorkoutTargetRaw>().ToDictionary(t => t.MuscleGroupId, t => t.Count);
                    workout.Sets = multi.Read<WorkoutSet>().ToArray();

                }
                return workout;
            }

        }
        public bool CreateWorkout(WorkoutDetails workout)
        {
            workout.Id = Guid.NewGuid();
            workout.Created = DateTimeOffset.Now;
            foreach(var set in workout.Sets ?? Enumerable.Empty<WorkoutSet>())
            {
                set.Id = Guid.NewGuid();
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Workout(Id, UserId, Time) VALUES(@Id, @UserId, @Time)", workout, tran);
                    if (workout.Sets != null)
                    {
                        conn.Execute("INSERT INTO WorkoutSet(Id,WorkoutId,[Index],ExerciseId,Reps,Weights,OneRepMax) VALUES(@Id,@WorkoutId,@Index,@ExerciseId,@Reps,@Weights,@OneRepMax)", workout.Sets.Select((s, i) => new
                        {
                            s.Id,
                            WorkoutId = workout.Id,
                            Index = i,
                            s.ExerciseId,
                            s.Reps,
                            s.Weights,
                            s.OneRepMax
                        }), tran);
                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    workout.Id = Guid.Empty;
                    foreach (var set in workout.Sets ?? Enumerable.Empty<WorkoutSet>())
                    {
                        set.Id = Guid.Empty;
                    }
                    tran.Rollback();
                    throw;
                }
            }
        }
        public bool UpdateWorkout(WorkoutDetails workout)
        {
            foreach (var set in workout.Sets.Where(r => r.Id == Guid.Empty))
            {
                set.Id = Guid.NewGuid();
            }
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM WorkoutSet WHERE WorkoutId=@Id", new { workout.Id }, tran);

                    conn.Execute("UPDATE Workout SET Time=@Time WHERE Id=@Id", workout, tran);
                    conn.Execute("INSERT INTO WorkoutSet(Id,WorkoutId,[Index],ExerciseId,Reps,Weights,OneRepMax) VALUES(@Id,@WorkoutId,@Index,@ExerciseId,@Reps,@Weights,@OneRepMax)", workout.Sets.Select((s, i) => new
                    {
                        s.Id,
                        WorkoutId = workout.Id,
                        Index = i,
                        s.ExerciseId,
                        s.Reps,
                        s.Weights,
                        s.OneRepMax
                    }), tran);
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
        public bool DeleteWorkout(Workout workout)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Workout SET Deleted=@Deleted WHERE Id=@Id", new { workout.Id, Deleted = DateTimeOffset.Now }, tran);
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
        public bool RestoreWorkout(Guid id, out WorkoutDetails workout)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TrainingGoalDetails> GetTrainingGoals(Guid userId)
        {
            return GetTrainingGoals("UserId=@userId AND Deleted IS NULL", new { userId });
        }
        public TrainingGoalDetails GetTrainingGoal(Guid id)
        {
            return GetTrainingGoals("Id=@id", new { id }).FirstOrDefault();
        }
        private IEnumerable<TrainingGoalDetails> GetTrainingGoals(string filter, object parameters)
        {
            var sql = $@"
SELECT * FROM TrainingGoal WHERE {filter};
SELECT TGE.*, E.Name AS ExerciseName FROM TrainingGoalExercise TGE 
JOIN Exercise E ON E.Id=TGE.ExerciseId 
WHERE TGE.TrainingGoalId IN (SELECT Id FROM TrainingGoal WHERE {filter}) ORDER By TGE.[Index];";

            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, parameters))
            {
                var goals = multi.Read<TrainingGoalDetails>().ToList();
                var exercises = multi.Read<TrainingGoalExerciseRaw>().ToList();
                foreach (var goal in goals)
                {
                    goal.Exercises = exercises.Where(p => p.TrainingGoalId == goal.Id).ToArray();
                }
                return goals;
            }
        }
        public void CreateTrainingGoal(TrainingGoalDetails goal)
        {
            goal.Id = Guid.NewGuid();
            foreach (var exercise in goal.Exercises)
            {
                exercise.Id = Guid.NewGuid();
            }

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO TrainingGoal(Id,UserId,Name,Created) VALUES(@Id,@UserId,@Name,@Created)", goal, tran);
                    conn.Execute("INSERT TrainingGoalExercise(Id,TrainingGoalId,[Index],ExerciseId,Sets,Reps,Frequency) VALUES (@Id,@TrainingGoalId,@index,@ExerciseId,@Sets,@Reps,@Frequency)", goal.Exercises.Select((e,index) => new
                    {
                        e.Id,
                        TrainingGoalId = goal.Id,
                        index,
                        e.ExerciseId,
                        e.Sets,
                        e.Reps,
                        e.Frequency
                    }), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void UpdateTrainingGoal(TrainingGoalDetails goal)
        {
            foreach (var exercise in goal.Exercises)
            {
                exercise.Id = Guid.NewGuid();
            }

            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM TrainingGoalExercise WHERE TrainingGoalId=@Id", goal, tran);

                    conn.Execute("UPDATE TrainingGoal SET Name=@Name WHERE Id=@Id", goal, tran);
                    conn.Execute("INSERT TrainingGoalExercise(Id,TrainingGoalId,[Index],ExerciseId,Sets,Reps,Frequency) VALUES (@Id,@TrainingGoalId,@index,@ExerciseId,@Sets,@Reps,@Frequency)", goal.Exercises.Select((e, index) => new
                    {
                        e.Id,
                        TrainingGoalId = goal.Id,
                        index,
                        e.ExerciseId,
                        e.Sets,
                        e.Reps,
                        e.Frequency
                    }), tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void ActivateTrainingGoal(TrainingGoal goal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE TrainingGoal SET Active=0 WHERE UserId=@UserId", new { goal.UserId }, tran);
                    conn.Execute("UPDATE TrainingGoal SET Active=1 WHERE Id=@Id", new { goal.Id }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void DeleteTrainingGoal(TrainingGoal goal)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE TrainingGoal SET Deleted=@Deleted WHERE Id=@Id", new { goal.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        class WorkoutTargetRaw
        {
            public Guid WorkoutId { get; set; }
            public Guid MuscleGroupId { get; set; }
            public int Count { get; set; }
        }
        class WorkoutSetRaw : WorkoutSet
        {
            public Guid WorkoutId { get; set; }
            public int Index { get; set; }
        }
        class RoutineExerciseRaw : RoutineExercise
        {
            public Guid RoutineWorkoutId { get; set; }
            public int Index { get; set; }
        }
        class ExerciseTargetRaw
        {
            public Guid ExerciseId { get; set; }
            public Guid MuscleGroupId { get; set; }
        }
        class TrainingGoalExerciseRaw : TrainingGoalExercise
        {
            public Guid TrainingGoalId { get; set; }
        }
    }
}
