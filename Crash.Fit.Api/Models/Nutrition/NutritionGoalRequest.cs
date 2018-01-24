using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class NutritionGoalRequest
    {
        public string Name { get; set; }
        public NutritionGoalPeriodRequest[] Periods { get; set; }
    }
    public class NutritionGoalPeriodRequest
    {
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
        public NutrientValue[] Nutrients { get; set; }

        public class NutrientValue
        {
            public int NutrientId { get; set; }
            public decimal? Min { get; set; }
            public decimal? Max { get; set; }
        }
    }
}
