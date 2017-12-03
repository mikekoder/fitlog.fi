using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit.Training
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
