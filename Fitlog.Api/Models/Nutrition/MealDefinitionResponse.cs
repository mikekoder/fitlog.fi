using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Nutrition
{
    public class MealDefinitionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? StartHour { get; set; }
        public int? StartMinute { get; set; }
        public int? EndHour { get; set; }
        public int? EndMinute { get; set; }
    }
}
