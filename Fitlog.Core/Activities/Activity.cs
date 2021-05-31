using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Activities
{
    public class Activity : Entity
    {
        public string Name { get; set; }
        /// <summary>
        /// kcal/kg/minute
        /// </summary>
        public decimal EnergyExpenditure { get; set; }
    }
}
