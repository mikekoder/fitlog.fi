using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class WorkoutMinimal : Entity
    {
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
    }
    public class WorkoutDetails : WorkoutMinimal
    {
       
        public WorkoutSet[] Sets { get; set; }
    }
}
