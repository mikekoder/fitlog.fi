using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public interface ITrainingRepository
    {
        IEnumerable<MuscleGroup> GetMuscleGroups();
        IEnumerable<Equipment> GetEquipment();
        IEnumerable<ExerciseDetails> SearchExercises(string[] nameTokens, Guid? muscleGroupId, Guid? equipmentId, Guid? userId, DateTimeOffset start1rm);
        IEnumerable<ExerciseDetails> SearchUserExercises(Guid userId, DateTimeOffset start1RM);
        IEnumerable<ExerciseDetails> ListLatestExercises(Guid userId, DateTimeOffset start1RM);
        IEnumerable<ExerciseDetails> ListMostUsedExercises(Guid userId, DateTimeOffset start1RM);
        ExerciseDetails GetExercise(Guid id, Guid userId, DateTimeOffset start1RM);
        IEnumerable<ExerciseDetails> GetExercises(IEnumerable<Guid> ids, Guid userId, DateTimeOffset start1RM);
        void CreateExercise(ExerciseDetails exercise);
        void UpdateExercise(ExerciseDetails exercise);
        void DeleteExercise(Exercise exercise);
        void RestoreExercise(Guid id, out ExerciseDetails exercise);

        IEnumerable<WorkoutDetails> SearchWorkouts(Guid userId, DateTimeOffset start, DateTimeOffset end);
        
        WorkoutDetails GetWorkout(Guid id);
        void CreateWorkout(WorkoutDetails workout);
        void UpdateWorkout(WorkoutDetails workout);
        void DeleteWorkout(Workout workout);
        void RestoreWorkout(Guid id, out WorkoutDetails workout);

        IEnumerable<RoutineSummary> SearchRoutines(Guid userId);
        RoutineDetails GetRoutine(Guid id);
        void CreateRoutine(RoutineDetails routine);
        void UpdateRoutine(RoutineDetails routine);
        void DeleteRoutine(Routine routine);
        void RestoreRoutine(Guid id, out RoutineDetails routine);
        void ActivateRoutine(Guid userId, Guid routineId);
        IEnumerable<TrainingGoalDetails> GetTrainingGoals(Guid userId);
        TrainingGoalDetails GetTrainingGoal(Guid id);
        void CreateTrainingGoal(TrainingGoalDetails goal);
        void UpdateTrainingGoal(TrainingGoalDetails goal);
        void ActivateTrainingGoal(TrainingGoal goal);
        void DeleteTrainingGoal(TrainingGoal goal);     
        void SaveOneRepMaxs(IEnumerable<OneRepMax> maxs);
        IEnumerable<OneRepMax> GetOneRepMaxs(Guid userId, DateTimeOffset start);
        IEnumerable<OneRepMax> GetOneRepMaxHistory(Guid exerciseId, Guid userId, DateTimeOffset start, DateTimeOffset end);
        IEnumerable<ExerciseVolume> GetExerciseVolumeHistory(Guid exerciseId, Guid userId, DateTimeOffset start, DateTimeOffset end);
        ExerciseImageDetails GetExerciseImage(Guid id, Guid imageId);
        void TransferExerciseData(Guid userId, Guid fromExerciseId, Guid toExerciseId, bool transferWorkouts = true, bool transferRoutines = true, bool transfer1rm = true);
    }
}
