using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Nutrition
{
    public class FavouriteMeal
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MealId { get; set; }
        public string Name { get; set; }
    }
}
