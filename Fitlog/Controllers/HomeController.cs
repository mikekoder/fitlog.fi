using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Fitlog.Nutrition;
using Fitlog.Api.Models.Home;
using Fitlog.Logging;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace Fitlog.Web.Controllers
{
    public class HomeController : ApiControllerBase
    {
        public HomeController(ILogRepository logger, IMapper mapper) : base(logger, mapper)
        {
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

    }
}
