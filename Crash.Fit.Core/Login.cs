﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit
{
    public class Login
    {
        public Guid UserId { get; set; }
        public string Provider { get; set; }
        public string ProviderKey { get; set; }
    }
}
