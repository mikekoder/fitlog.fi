﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Crash.Fit.Web.Models.Auth
{
    public class User: IdentityUser<Guid>
    {
    }
}
