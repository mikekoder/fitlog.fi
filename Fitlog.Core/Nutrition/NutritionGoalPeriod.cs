using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Nutrition
{
    public class NutritionGoalPeriod
    {
        public Guid Id { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool ExerciseDay { get; set; }
        public bool RestDay { get; set; }
        public bool WholeDay { get; set; }
        public Guid[] MealDefinitions { get; set; }
        public NutritionGoalValue[] Nutrients { get; set; }
    }
}
