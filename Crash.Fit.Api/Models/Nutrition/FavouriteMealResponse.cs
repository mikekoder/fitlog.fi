using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FavouriteMealResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MealId { get; set; }
        public string Name { get; set; }
        public MealDetailsResponse Meal { get; set; }
    }
}
