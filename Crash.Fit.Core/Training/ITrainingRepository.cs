using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public interface ITrainingRepository
    {
        IEnumerable<MuscleGroup> GetMuscleGroups();

        IEnumerable<ExerciseMinimal> SearchExercises(string[] nameTokens, Guid? userId);
        IEnumerable<ExerciseMinimal> SearchUserExercises(Guid userId);
        ExerciseDetails GetExercise(Guid id);
        bool CreateExercise(ExerciseDetails exercise);
        bool UpdateExercise(ExerciseDetails exercise);
        bool DeleteExercise(ExerciseMinimal exercise);
        bool RestoreExercise(Guid id, out ExerciseDetails exercise);

        IEnumerable<WorkoutSummary> SearchWorkouts(Guid userId, DateTimeOffset start, DateTimeOffset end);
        WorkoutDetails GetWorkout(Guid id);
        bool CreateWorkout(WorkoutDetails workout);
        bool UpdateWorkout(WorkoutDetails workout);
        bool DeleteWorkout(WorkoutMinimal workout);
        bool RestoreWorkout(Guid id, out WorkoutDetails workout);

        IEnumerable<RoutineMinimal> SearchRoutines(Guid userId);
        RoutineDetails GetRoutine(Guid id);
        bool CreateRoutine(RoutineDetails routine);
        bool UpdateRoutine(RoutineDetails routine);
        bool DeleteRoutine(RoutineMinimal routine);
        bool RestoreRoutine(Guid id, out RoutineDetails routine);
    }
}
