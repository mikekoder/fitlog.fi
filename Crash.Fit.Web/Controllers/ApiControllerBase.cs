using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Controllers
{
    public class ApiControllerBase : Controller
    {
        protected Guid CurrentUserId
        {
            get
            {
                return new Guid();
            }
        }
    }
}
