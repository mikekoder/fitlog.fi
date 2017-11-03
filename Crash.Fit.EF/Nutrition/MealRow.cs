using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class MealRow
    {
        public MealRow()
        {
            Nutrients = new HashSet<MealRowNutrient>();
        }

        public Guid Id { get; set; }
        public Guid MealId { get; set; }
        public int Index { get; set; }
        public Guid FoodId { get; set; }
        public Guid? PortionId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal Weight { get; set; }

        public Food Food { get; set; }
        public Meal Meal { get; set; }
        public FoodPortion Portion { get; set; }
        public ICollection<MealRowNutrient> Nutrients { get; set; }
    }
}
