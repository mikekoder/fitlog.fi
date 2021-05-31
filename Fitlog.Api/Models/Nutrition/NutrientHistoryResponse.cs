using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Nutrition
{
    public class NutrientHistoryResponse
    {
        public DateTimeOffset Date { get; set; }
        public Dictionary<int, decimal> Nutrients { get; set; }
        public decimal? EnergyExpenditure { get; set; }
    }
}
