using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class FoodPortion
    {
        public FoodPortion()
        {
            MealRow = new HashSet<MealRow>();
        }

        public Guid Id { get; set; }
        public Guid FoodId { get; set; }
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public decimal? Amount { get; set; }

        public Food Food { get; set; }
        public ICollection<MealRow> MealRow { get; set; }
    }
}
