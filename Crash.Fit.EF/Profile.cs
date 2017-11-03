using F = Crash.Fit.EF.Feedback;
using Crash.Fit.EF.Measurements;
using Crash.Fit.EF.Nutrition;
using Crash.Fit.EF.Training;
using System;
using System.Collections.Generic;

namespace Crash.Fit.EF
{
    public partial class Profile
    {
        public Profile()
        {
            
            Exercises = new HashSet<Exercise>();
            Feedbacks = new HashSet<F.Feedback>();
            FeedbackComments = new HashSet<F.FeedbackComment>();
            FeedbackVotes = new HashSet<F.FeedbackVote>();
            Foods = new HashSet<Food>();
            Meals = new HashSet<Meal>();
            MealDefinitions = new HashSet<MealDefinition>();
            Measures = new HashSet<Measure>();
            Measurements = new HashSet<Measurement>();
            NutrientSettings = new HashSet<NutrientSettings>();
            NutritionGoals = new HashSet<NutritionGoal>();
            OneRepMaxs = new HashSet<OneRepMax>();
            Routines = new HashSet<Routine>();
            TrainingGoals = new HashSet<TrainingGoal>();
            Workouts = new HashSet<Workout>();
        }

        public Guid UserId { get; set; }
        public DateTime? DoB { get; set; }
        public string Gender { get; set; }
        public decimal? Rmr { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string RefreshToken { get; set; }

        
        public ICollection<Exercise> Exercises { get; set; }
        public ICollection<F.Feedback> Feedbacks { get; set; }
        public ICollection<F.FeedbackComment> FeedbackComments { get; set; }
        public ICollection<F.FeedbackVote> FeedbackVotes { get; set; }
        public ICollection<Food> Foods { get; set; }
        public ICollection<Meal> Meals { get; set; }
        public ICollection<MealDefinition> MealDefinitions { get; set; }
        public ICollection<Measure> Measures { get; set; }
        public ICollection<Measurement> Measurements { get; set; }
        public ICollection<NutrientSettings> NutrientSettings { get; set; }
        public ICollection<NutritionGoal> NutritionGoals { get; set; }
        public ICollection<OneRepMax> OneRepMaxs { get; set; }
        public ICollection<Routine> Routines { get; set; }
        public ICollection<TrainingGoal> TrainingGoals { get; set; }
        public ICollection<Workout> Workouts { get; set; }
    }
}
