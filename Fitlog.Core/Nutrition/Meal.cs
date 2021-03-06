﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitlog.Nutrition
{
    public class Meal : Entity
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public Guid? DefinitionId { get; set; }
    }
    public class MealDetails : Meal
    {
        public Dictionary<int,decimal> Nutrients { get; set; }
        public MealRow[] Rows { get; set; }
    }
}
