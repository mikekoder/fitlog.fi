using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class AddMealRowRequest
    {
        public DateTimeOffset Date { get; set; }
        public Guid? MealDefinitionId { get; set; }
        public Guid? MealId { get; set; }
        public Guid FoodId { get; set; }
        public decimal Amount { get; set; }
        public Guid? PortionId { get; set; }
    }
}
