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
        public TrainingRepository(string connectionString) : base(connectionString)
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
        public IEnumerable<Equipment> GetEquipment()
        {
            var sql = @"SELECT * FROM Equipment";
            using (var conn = CreateConnection())
            {
                return conn.Query<Equipment>(sql).ToList();
            }
        }

        public IEnumerable<Exercise> SearchExercises(string[] nameTokens, Guid? muscleGroupId, Guid? equipmentId, Guid? userId)
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
            if (muscleGroupId.HasValue)
            {
                sql += " AND (ExerciseTarget.MuscleGroupId=@MuscleGroupId)";
                parameters.Add("MuscleGroupId", muscleGroupId.Value);
            }
            if (equipmentId.HasValue)
            {
                sql += " AND (ExerciseEquipment.EquipmentId=@EquipmentId)";
                parameters.Add("EquipmentId", equipmentId.Value);
            }

            sql = @"SELECT DISTINCT Exercise.* FROM Exercise
JOIN ExerciseTarget ON ExerciseTarget.ExerciseId=Exercise.Id
JOIN ExerciseEquipment ON ExerciseEquipment.ExerciseId=Exercise.Id
WHERE " + sql.Substring(5) + " ORDER BY Name";
            using (var conn = CreateConnection())
            {
                return conn.Query<Exercise>(sql, parameters);
            }
        }
        public IEnumerable<ExerciseDetails> SearchUserExercises(Guid userId, DateTimeOffset start1RM)
        {
            var filter = "UserId=@userId AND Deleted IS NULL";
            var sql = $@"
SELECT Exercise.*,(SELECT COUNT(*) FROM WorkoutSet WHERE ExerciseId=Exercise.Id) AS UsageCount, (SELECT MAX (Max) FROM OneRepMax WHERE ExerciseId=Exercise.Id AND Time >= @time AND UserId=@userId) AS OneRepMax FROM Exercise WHERE {filter};
SELECT * FROM ExerciseTarget WHERE Type='Primary' AND ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});
SELECT * FROM ExerciseTarget WHERE Type='Secondary' AND ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});
SELECT * FROM ExerciseEquipment WHERE ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId, time = start1RM }))
            {
                var exercises = multi.Read<ExerciseDetails>().ToList();
                var primaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var secondaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var equipments = multi.Read<ExerciseEquipmentRaw>().ToList();
                foreach (var exercise in exercises)
                {
                    exercise.Targets = primaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.SecondaryTargets = secondaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.Equipments = equipments.Where(e => e.ExerciseId == exercise.Id).Select(e => e.EquipmentId).ToArray();
                }
                return exercises;
            }
        }
        public IEnumerable<ExerciseDetails> ListLatestExercises(Guid userId, DateTimeOffset start1RM)
        {
            var filter = "(UserId=@userId OR UserId IS NULL) AND Deleted IS NULL";
            var sql = $@"
SELECT Exercise.*,
(SELECT COUNT(*) FROM WorkoutSet WHERE ExerciseId=Exercise.Id) AS UsageCount, 
(SELECT MAX (Max) FROM OneRepMax WHERE ExerciseId=Exercise.Id AND Time >= @time AND UserId=@userId) AS OneRepMax,
(SELECT MAX (Time) FROM Workout JOIN WorkoutSet ON WorkoutSet.WorkoutId=WorkoutId WHERE WorkoutSet.ExerciseId=Exercise.Id) AS LatestUse
FROM Exercise WHERE {filter}
ORDER BY LatestUse DESC;
SELECT * FROM ExerciseTarget WHERE Type='Primary' AND ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});
SELECT * FROM ExerciseTarget WHERE Type='Secondary' AND ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});
SELECT * FROM ExerciseEquipment WHERE ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId, time = start1RM }))
            {
                var exercises = multi.Read<ExerciseDetails>().ToList();
                var primaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var secondaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var equipments = multi.Read<ExerciseEquipmentRaw>().ToList();
                foreach (var exercise in exercises)
                {
                    exercise.Targets = primaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.SecondaryTargets = secondaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.Equipments = equipments.Where(e => e.ExerciseId == exercise.Id).Select(e => e.EquipmentId).ToArray();
                }
                return exercises;
            }
        }
        public IEnumerable<ExerciseDetails> ListMostUsedExercises(Guid userId, DateTimeOffset start1RM)
        {
            var filter = "(UserId=@userId OR UserId IS NULL) AND Deleted IS NULL";
            var sql = $@"
SELECT Exercise.*,
(SELECT COUNT(*) FROM WorkoutSet WHERE ExerciseId=Exercise.Id) AS UsageCount, 
(SELECT MAX (Max) FROM OneRepMax WHERE ExerciseId=Exercise.Id AND Time >= @time AND UserId=@userId) AS OneRepMax,
(SELECT MAX (Time) FROM Workout JOIN WorkoutSet ON WorkoutSet.WorkoutId=WorkoutId WHERE WorkoutSet.ExerciseId=Exercise.Id) AS LatestUse
FROM Exercise WHERE {filter}
ORDER BY UsageCount DESC;
SELECT * FROM ExerciseTarget WHERE Type='Primary' AND ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});
SELECT * FROM ExerciseTarget WHERE Type='Secondary' AND ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});
SELECT * FROM ExerciseEquipment WHERE ExerciseId IN (SELECT Id FROM Exercise WHERE {filter});";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { userId, time = start1RM }))
            {
                var exercises = multi.Read<ExerciseDetails>().ToList();
                var primaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var secondaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var equipments = multi.Read<ExerciseEquipmentRaw>().ToList();
                foreach (var exercise in exercises)
                {
                    exercise.Targets = primaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.SecondaryTargets = secondaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.Equipments = equipments.Where(e => e.ExerciseId == exercise.Id).Select(e => e.EquipmentId).ToArray();
                }
                return exercises;
            }
        }
        public ExerciseDetails GetExercise(Guid id)
        {
            var sql = @"
SELECT * FROM Exercise WHERE Id=@id;
SELECT MuscleGroupId FROM ExerciseTarget WHERE Type='Primary' AND ExerciseId=@id;
SELECT MuscleGroupId FROM ExerciseTarget WHERE Type='Secondary' AND ExerciseId=@id;
SELECT EquipmentId FROM ExerciseEquipment WHERE ExerciseId=@id;
SELECT * FROM ExerciseImage WHERE ExerciseId=@id;";
            using (var conn = CreateConnection())
                using(var multi = conn.QueryMultiple(sql,new { id }))
            {
                var exercise = multi.Read<ExerciseDetails>().SingleOrDefault();
                if(exercise != null)
                {
                    exercise.Targets = multi.Read<Guid>().ToArray();
                    exercise.SecondaryTargets = multi.Read<Guid>().ToArray();
                    exercise.Equipments = multi.Read<Guid>().ToArray();
                    exercise.Images = multi.Read<ExerciseImage>().ToArray();
                }
                return exercise;
            }
        }
        public IEnumerable<ExerciseDetails> GetExercises(IEnumerable<Guid> ids)
        {
            var sql = @"
SELECT * FROM Exercise WHERE Id IN @ids;
SELECT MuscleGroupId FROM ExerciseTarget WHERE Type='Primary' AND ExerciseId IN @ids;
SELECT MuscleGroupId FROM ExerciseTarget WHERE Type='Secondary' AND ExerciseId IN @ids;
SELECT * FROM ExerciseEquipment WHERE ExerciseId IN @ids;";
            using (var conn = CreateConnection())
            using (var multi = conn.QueryMultiple(sql, new { ids }))
            {
                var exercises = multi.Read<ExerciseDetails>().ToList();
                var primaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var secondaryTargets = multi.Read<ExerciseTargetRaw>().ToList();
                var equipments = multi.Read<ExerciseEquipmentRaw>().ToList();
                foreach (var exercise in exercises)
                {
                    exercise.Targets = primaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.SecondaryTargets = secondaryTargets.Where(t => t.ExerciseId == exercise.Id).Select(t => t.MuscleGroupId).ToArray();
                    exercise.Equipments = equipments.Where(e => e.ExerciseId == exercise.Id).Select(e => e.EquipmentId).ToArray();
                }
                return exercises;
            }
        }
        public void CreateExercise(ExerciseDetails exercise)
        {
            exercise.Id = Guid.NewGuid();
            exercise.Created = DateTimeOffset.Now;
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO Exercise(Id, UserId, Name,Created,PercentageBW) VALUES(@Id, @UserId, @Name,@Created,@PercentageBW)", exercise, tran);
                    if (exercise.Targets != null && exercise.Targets.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId,Type) VALUES(@ExerciseId,@MuscleGroupId,'Primary')", exercise.Targets.Select(t => new { ExerciseId = exercise.Id, MuscleGroupId = t }), tran);
                    }
                    if (exercise.SecondaryTargets != null && exercise.SecondaryTargets.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId,Type) VALUES(@ExerciseId,@MuscleGroupId,'Secondary')", exercise.SecondaryTargets.Select(t => new { ExerciseId = exercise.Id, MuscleGroupId = t }), tran);
                    }
                    if(exercise.Equipments != null && exercise.Equipments.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseEquipment(ExerciseId,EquipmentId) VALUES(@ExerciseId,@EquipmentId)", exercise.Equipments.Select(e => new { ExerciseId = exercise.Id, EquipmentId = e }), tran);
                    }
                    tran.Commit();
                }
                catch
                {
                    exercise.Id = Guid.Empty;
                    throw;
                }
            }
        }
        public void UpdateExercise(ExerciseDetails exercise)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM ExerciseTarget WHERE ExerciseId=@Id", new { exercise.Id }, tran);
                    conn.Execute("DELETE FROM ExerciseEquipment WHERE ExerciseId=@Id", new { exercise.Id }, tran);
                    conn.Execute("UPDATE Exercise SET Name=@Name,PercentageBW=@PercentageBW WHERE Id=@Id", exercise, tran);
                    if (exercise.Targets != null && exercise.Targets.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId,Type) VALUES(@ExerciseId,@MuscleGroupId,'Primary')", exercise.Targets.Select(t => new { ExerciseId = exercise.Id, MuscleGroupId = t }), tran);
                    }
                    if (exercise.SecondaryTargets != null && exercise.SecondaryTargets.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseTarget(ExerciseId,MuscleGroupId,Type) VALUES(@ExerciseId,@MuscleGroupId,'Secondary')", exercise.SecondaryTargets.Select(t => new { ExerciseId = exercise.Id, MuscleGroupId = t }), tran);
                    }
                    if (exercise.Equipments != null && exercise.Equipments.Length > 0)
                    {
                        conn.Execute("INSERT INTO ExerciseEquipment(ExerciseId,EquipmentId) VALUES(@ExerciseId,@EquipmentId)", exercise.Equipments.Select(e => new { ExerciseId = exercise.Id, EquipmentId = e }), tran);
                    }
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void DeleteExercise(Exercise exercise)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Exercise SET Deleted=@Deleted WHERE Id=@Id", new { exercise.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void RestoreExercise(Guid id, out ExerciseDetails exercise)
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
        public void CreateRoutine(RoutineDetails routine)
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
                    conn.Execute("INSERT INTO Routine(Id, UserId, Name,Created,Active) VALUES(@Id, @UserId, @Name,@Created,@Active)", routine, tran);
                    conn.Execute("INSERT INTO RoutineWorkout(Id,RoutineId,[Index],Name,Frequency) VALUES(@Id,@RoutineId,@Index,@Name,@Frequency)", routine.Workouts.Select((w, i) => new
                    {
                        w.Id,
                        RoutineId = routine.Id,
                        Index = i,
                        w.Name,
                        w.Frequency
                    }), tran);
                    conn.Execute("INSERT INTO RoutineExercise(Id,RoutineWorkoutId,[Index],ExerciseId,Sets,Reps,LoadFrom,LoadTo) VALUES(newid(),@RoutineWorkoutId,@Index,@ExerciseId,@Sets,@Reps,@LoadFrom,@LoadTo)", routine.Workouts.SelectMany(w => w.Exercises.Select((e,i) => new
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
                }
                catch
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
        public void UpdateRoutine(RoutineDetails routine)
        {
            var workoutIds = routine.Workouts.Select(w => w.Id).ToArray();
            
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("DELETE FROM RoutineExercise WHERE RoutineWorkoutId IN (SELECT Id FROM RoutineWorkout WHERE RoutineId=@Id)", new { routine.Id }, tran);               
                    conn.Execute("DELETE FROM RoutineWorkout WHERE RoutineId=@Id", new { routine.Id }, tran);

                    for(var i = 0; i< routine.Workouts.Length; i++)
                    {
                        var workout = routine.Workouts[i];
                        if(workout.Id == Guid.Empty)
                        {
                            workout.Id = Guid.NewGuid();
                            
                        }
                        conn.Execute("INSERT INTO RoutineWorkout(Id,RoutineId,[Index],Name,Frequency) VALUES(@Id,@RoutineId,@Index,@Name,@Frequency)", new
                        {
                            workout.Id,
                            RoutineId = routine.Id,
                            Index = i,
                            workout.Name,
                            workout.Frequency
                        }, tran);
                    }

                    conn.Execute("UPDATE Routine SET Name=@Name WHERE Id=@Id", routine, tran);
                    
                    conn.Execute("INSERT INTO RoutineExercise(Id,RoutineWorkoutId,[Index],ExerciseId,Sets,Reps,LoadFrom,LoadTo) VALUES(newid(),@RoutineWorkoutId,@Index,@ExerciseId,@Sets,@Reps,@LoadFrom,@LoadTo)", routine.Workouts.SelectMany(w => w.Exercises.Select((e, i) => new
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
                }
                catch
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
        public void DeleteRoutine(Routine routine)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Routine SET Deleted=@Deleted WHERE Id=@Id", new { routine.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }    
        public void RestoreRoutine(Guid id, out RoutineDetails routine)
        {
            throw new NotImplementedException();
        }
        public void ActivateRoutine(Guid userId, Guid routineId)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Routine SET Active=0 WHERE UserId=@userId", new { userId }, tran);
                    conn.Execute("UPDATE Routine SET Active=1 WHERE Id=@routineId", new { routineId }, tran);
                    tran.Commit();
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
        public void CreateWorkout(WorkoutDetails workout)
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
                    conn.Execute("INSERT INTO Workout(Id, UserId, Time, Duration) VALUES(@Id, @UserId, @Time, @Duration)", workout, tran);
                    if (workout.Sets != null)
                    {
                        conn.Execute("INSERT INTO WorkoutSet(Id,WorkoutId,[Index],ExerciseId,Reps,Weights,WeightsBW,Load,LoadBW) VALUES(@Id,@WorkoutId,@Index,@ExerciseId,@Reps,@Weights,@WeightsBW,@Load,@LoadBW)", workout.Sets.Select((s, i) => new
                        {
                            s.Id,
                            WorkoutId = workout.Id,
                            Index = i,
                            s.ExerciseId,
                            s.Reps,
                            s.Weights,
                            s.WeightsBW,
                            s.Load,
                            s.LoadBW
                        }), tran);
                    }
                    tran.Commit();
                }
                catch
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
        public void UpdateWorkout(WorkoutDetails workout)
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

                    conn.Execute("UPDATE Workout SET Time=@Time, Duration=@Duration WHERE Id=@Id", workout, tran);
                    conn.Execute("INSERT INTO WorkoutSet(Id,WorkoutId,[Index],ExerciseId,Reps,Weights,WeightsBW,Load,LoadBW) VALUES(@Id,@WorkoutId,@Index,@ExerciseId,@Reps,@Weights,@WeightsBW,@Load,@LoadBW)", workout.Sets.Select((s, i) => new
                    {
                        s.Id,
                        WorkoutId = workout.Id,
                        Index = i,
                        s.ExerciseId,
                        s.Reps,
                        s.Weights,
                        s.WeightsBW,
                        s.Load,
                        s.LoadBW
                    }), tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void DeleteWorkout(Workout workout)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("UPDATE Workout SET Deleted=@Deleted WHERE Id=@Id", new { workout.Id, Deleted = DateTimeOffset.Now }, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public void RestoreWorkout(Guid id, out WorkoutDetails workout)
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
                catch
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
                catch
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
        public void SaveOneRepMaxs(IEnumerable<OneRepMax> maxs)
        {
            using (var conn = CreateConnection())
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    conn.Execute("INSERT INTO OneRepMax(Id,UserId,ExerciseId,Time,Max,MaxBW,MaxInclBW) VALUES(newid(),@UserId,@ExerciseId,@Time,@Max,@MaxBW,@MaxInclBW)", maxs, tran);
                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        public IEnumerable<OneRepMax> GetOneRepMaxs(Guid userId, DateTimeOffset start)
        {
            using (var conn = CreateConnection())
            {
                var maxs = conn.Query<OneRepMax>("SELECT * FROM OneRepMax WHERE UserId=@userId AND Time >= @start", new { userId, start });
                return maxs.GroupBy(m => m.ExerciseId).Select(m => m.OrderByDescending(m2 => m2.Time).First());
            }
        }
        public IEnumerable<OneRepMax> GetOneRepMaxHistory(Guid exerciseId, Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<OneRepMax>("SELECT * FROM OneRepMax WHERE ExerciseId=@exerciseId AND UserId=@userId AND Time >= @start AND Time <= @end ORDER BY Time", new { exerciseId, userId, start, end });
            }
        }
        public IEnumerable<ExerciseVolume> GetExerciseVolumeHistory(Guid exerciseId, Guid userId, DateTimeOffset start, DateTimeOffset end)
        {
            using (var conn = CreateConnection())
            {
                return conn.Query<ExerciseVolume>(@"SELECT W.UserId, W.Time, EV.ExerciseId, EV.Volume AS TotalVolume FROM Workout W
JOIN ExerciseVolume EV ON EV.WorkoutId = W.Id 
WHERE ExerciseId=@exerciseId AND UserId=@userId AND Time >= @start AND Time <= @end ORDER BY Time", new { exerciseId, userId, start, end });
            }
        }
        public ExerciseImageDetails GetExerciseImage(Guid exerciseId, Guid imageId)
        {
            using (var conn = CreateConnection())
            {
                return conn.QuerySingle<ExerciseImageDetails>(@"SELECT * FROM ExerciseImage WHERE Id=@imageId AND ExerciseId=@exerciseID", new { exerciseId, imageId });
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
        class ExerciseEquipmentRaw
        {
            public Guid ExerciseId { get; set; }
            public Guid EquipmentId { get; set; }
        }
        class TrainingGoalExerciseRaw : TrainingGoalExercise
        {
            public Guid TrainingGoalId { get; set; }
        }
        
    }
}
