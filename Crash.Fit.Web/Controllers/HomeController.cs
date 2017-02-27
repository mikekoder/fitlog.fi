using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Crash.Fit.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAntiforgery antiforgery;
        public HomeController(IAntiforgery antiforgery)
        {
            this.antiforgery = antiforgery;
        }
        public IActionResult Index()
        {
            var tokens = antiforgery.GetAndStoreTokens(HttpContext);
            ViewData["X-CSRF-TOKEN"] = tokens.RequestToken;
            return View();
        }
    }
}
