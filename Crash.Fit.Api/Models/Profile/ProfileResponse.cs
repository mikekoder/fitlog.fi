﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Api.Models.Profile
{
    public class ProfileResponse
    {
        public Guid UserId { get; set; }
        public DateTime? DoB { get; set; }
        public string Gender { get; set; }
        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Rmr { get; set; }
    }
}
