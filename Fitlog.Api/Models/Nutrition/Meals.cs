using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Nutrition
{
    public enum Meals
    {
        None = 0,
        Breakfast = 1 << 1,
        Lunch = 1 << 2,
        Dinner = 1 << 3,
        Supper = 1 << 4,
        Snacks = 1 << 5
    }
}
