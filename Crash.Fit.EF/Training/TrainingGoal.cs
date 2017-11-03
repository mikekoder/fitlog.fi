using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Training
{
    public partial class TrainingGoal
    {
        public TrainingGoal()
        {
            Exercises = new HashSet<TrainingGoalExercise>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Deleted { get; set; }

        public Profile User { get; set; }
        public ICollection<TrainingGoalExercise> Exercises { get; set; }
    }
}
