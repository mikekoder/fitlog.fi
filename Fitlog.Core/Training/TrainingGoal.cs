using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Training
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
