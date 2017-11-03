using System;
using System.Collections.Generic;

namespace Crash.Fit.EF.Measurements
{
    public partial class Measure
    {
        public Measure()
        {
            Measurement = new HashSet<Measurement>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }

        public Profile User { get; set; }
        public ICollection<Measurement> Measurement { get; set; }
    }
}
