using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class Meal
    {
        public Meal()
        {
            Nutrients = new HashSet<MealNutrient>();
            Rows = new HashSet<MealRow>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Deleted { get; set; }
        public Guid? DefinitionId { get; set; }

        public MealDefinition Definition { get; set; }
        public Profile User { get; set; }
        public ICollection<MealNutrient> Nutrients { get; set; }
        public ICollection<MealRow> Rows { get; set; }
    }
}
