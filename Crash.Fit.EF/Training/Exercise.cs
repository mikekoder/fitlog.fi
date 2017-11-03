using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class Exercise
    {
        public Exercise()
        {
            Targets = new HashSet<ExerciseTarget>();
            OneRepMax = new HashSet<OneRepMax>();
            RoutineExercise = new HashSet<RoutineExercise>();
            TrainingGoalExercise = new HashSet<TrainingGoalExercise>();
            WorkoutSet = new HashSet<WorkoutSet>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public int? BodyWeightPercentage { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }

        public Profile User { get; set; }
        public ICollection<ExerciseTarget> Targets { get; set; }
        public ICollection<OneRepMax> OneRepMax { get; set; }
        public ICollection<RoutineExercise> RoutineExercise { get; set; }
        public ICollection<TrainingGoalExercise> TrainingGoalExercise { get; set; }
        public ICollection<WorkoutSet> WorkoutSet { get; set; }
    }
}
