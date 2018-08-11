using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Api.Models.Nutrition
{
    public class NutrientHistoryResponse
    {
        public DateTimeOffset Date { get; set; }
        public Dictionary<int, decimal> Nutrients { get; set; }
    }
}
