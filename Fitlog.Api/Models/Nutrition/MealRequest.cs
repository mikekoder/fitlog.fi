using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class MealRequest
    {
        public DateTimeOffset Date { get; set; }
        public string Time { get; set; }
        public Guid? DefinitionId { get; set; }
        public MealRowModel[] Rows { get; set; }
    }
}
