using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Nutrition
{
    public class Meal : Entity
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public Guid? DefinitionId { get; set; }
    }
    public class MealDetails : Meal
    {
        public NutrientAmount[] Nutrients { get; set; }
        public MealRow[] Rows { get; set; }
    }
}
