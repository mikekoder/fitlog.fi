using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public interface ITrainingRepository
    {
        IEnumerable<MuscleGroup> GetMuscleGroups();

        IEnumerable<Exercise> SearchExercises(string[] nameTokens, Guid? userId);
        IEnumerable<ExerciseDetails> SearchUserExercises(Guid userId);
        ExerciseDetails GetExercise(Guid id);
        bool CreateExercise(ExerciseDetails exercise);
        bool UpdateExercise(ExerciseDetails exercise);
        bool DeleteExercise(Exercise exercise);
        bool RestoreExercise(Guid id, out ExerciseDetails exercise);

        IEnumerable<WorkoutDetails> SearchWorkouts(Guid userId, DateTimeOffset start, DateTimeOffset end);
        WorkoutDetails GetWorkout(Guid id);
        bool CreateWorkout(WorkoutDetails workout);
        bool UpdateWorkout(WorkoutDetails workout);
        bool DeleteWorkout(Workout workout);
        bool RestoreWorkout(Guid id, out WorkoutDetails workout);

        IEnumerable<RoutineSummary> SearchRoutines(Guid userId);
        RoutineDetails GetRoutine(Guid id);
        bool CreateRoutine(RoutineDetails routine);
        bool UpdateRoutine(RoutineDetails routine);
        bool DeleteRoutine(Routine routine);
        bool RestoreRoutine(Guid id, out RoutineDetails routine);
        bool ActivateRoutine(Guid userId, Guid routineId);
    }
}
