using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Nutrition
{
    public class FoodResponse
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public Guid? NutrientPortionId { get; set; }
        public string Ean { get; set; }
    }
}
