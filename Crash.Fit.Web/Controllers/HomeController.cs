using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Crash.Fit.Nutrition;
using Crash.Fit.Api.Models.Home;
using Crash.Fit.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Crash.Fit.Web.Controllers
{
    public class HomeController : ApiControllerBase
    {
        public HomeController(ILogRepository logger):base(logger)
        {
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

    }
}
