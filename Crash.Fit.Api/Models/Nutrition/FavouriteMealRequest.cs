using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class FavouriteMealRequest
    {
        public Guid MealId { get; set; }
        public string Name { get; set; }
    }
}
