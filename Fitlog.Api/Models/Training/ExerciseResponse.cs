﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Api.Models.Training
{
    public class ExerciseResponse
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public decimal? PercentageBW { get; set; }
    }
}
