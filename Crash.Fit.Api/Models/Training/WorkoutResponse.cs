using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Training
{
    public class WorkoutResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Name { get; set; }
    }
}
