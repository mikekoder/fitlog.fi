using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Controllers
{
    public class ApiControllerBase : Controller
    {
        protected Guid CurrentUserId
        {
            get
            {
                var claim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrWhiteSpace(claim))
                {
                    return Guid.Parse(claim);
                }
                return Guid.Empty;
            }
        }
    }
}
