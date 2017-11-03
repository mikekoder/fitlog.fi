using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Nutrition
{
    public partial class RecipeIngredient
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public int Index { get; set; }
        public Guid FoodId { get; set; }
        public decimal Quantity { get; set; }
        public Guid? PortionId { get; set; }
        public decimal Weight { get; set; }

        public Food Food { get; set; }
        public Food Recipe { get; set; }
    }
}
