﻿using Crash.Fit.Nutrition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class RecipeResponse
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public bool IsRecipe { get; set; }
        public RecipeIngredient[] Ingredients { get; set; }
        public Portion[] Portions { get; set; }
        public Dictionary<Guid,decimal> Nutrients { get; set; }
    }
}
