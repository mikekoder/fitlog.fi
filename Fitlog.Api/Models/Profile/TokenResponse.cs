﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Fitlog.Api.Models.Profile
{
    public class TokenResponse
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
        public DateTimeOffset Expires { get; set; }
    }
}
