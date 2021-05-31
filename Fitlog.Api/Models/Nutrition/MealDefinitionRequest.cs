using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Nutrition
{
    public class MealDefinitionRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
