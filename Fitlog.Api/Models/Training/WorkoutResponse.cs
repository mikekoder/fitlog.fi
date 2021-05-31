using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Training
{
    public class WorkoutResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Time { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
    }
}
