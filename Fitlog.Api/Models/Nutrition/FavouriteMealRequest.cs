using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Nutrition
{
    public class FavouriteMealRequest
    {
        public Guid MealId { get; set; }
        public string Name { get; set; }
    }
}
