using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Training
{
    public class TrainingGoal : Entity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
    public class TrainingGoalDetails : TrainingGoal
    {
        public TrainingGoalExercise[] Exercises { get; set; }
    }
}
