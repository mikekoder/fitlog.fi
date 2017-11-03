using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class Nutrient
    {
        public Nutrient()
        {
            DailyIntakes = new HashSet<DailyIntake>();
            FoodNutrient = new HashSet<FoodNutrient>();
            MealNutrient = new HashSet<MealNutrient>();
            MealRowNutrient = new HashSet<MealRowNutrient>();
            NutrientSettings = new HashSet<NutrientSettings>();
            NutritionGoalValue = new HashSet<NutritionGoalValue>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Unit { get; set; }
        public string FineliId { get; set; }
        public string FineliClass { get; set; }
        public string FineliGroup { get; set; }
        public int? Precision { get; set; }
        public int? DefaultOrder { get; set; }
        public bool? DefaultHideSummary { get; set; }
        public bool? DefaultHideDetails { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public bool Computed { get; set; }

        public ICollection<DailyIntake> DailyIntakes { get; set; }
        public ICollection<FoodNutrient> FoodNutrient { get; set; }
        public ICollection<MealNutrient> MealNutrient { get; set; }
        public ICollection<MealRowNutrient> MealRowNutrient { get; set; }
        public ICollection<NutrientSettings> NutrientSettings { get; set; }
        public ICollection<NutritionGoalValue> NutritionGoalValue { get; set; }
    }
}
