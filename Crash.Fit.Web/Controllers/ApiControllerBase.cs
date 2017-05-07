using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using Microsoft.AspNetCore.Http;
using Crash.Fit.Logging;

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

        private readonly ILogRepository logger;
        public ApiControllerBase(ILogRepository logger)
        {
            this.logger = logger;
        }


        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var method = context.HttpContext.Request.Method;
                var path = context.HttpContext.Request.Path;
                var query = context.HttpContext.Request.QueryString.Value;
                var body = GetRequestBody(context.HttpContext.Request);
                var userId = CurrentUserId;

                logger.LogException(CurrentUserId, method, path + query, body, context.Exception);
            }
            base.OnActionExecuted(context);
        }
        private string GetRequestBody(HttpRequest request)
        {
            var ms = new MemoryStream();
            request.Body.Position = 0;
            request.Body.CopyTo(ms);
            using (var reader = new StreamReader(ms))
            {
                ms.Position = 0;
                return reader.ReadToEnd();
            }
        }
    }
}
