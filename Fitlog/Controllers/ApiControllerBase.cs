using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using Microsoft.AspNetCore.Http;
using Fitlog.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;
using AutoMapper;

namespace Fitlog.Web.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        protected readonly ILogRepository Logger;
        protected readonly IMapper Mapper;
        public ApiControllerBase(ILogRepository logger, IMapper mapper)
        {
            this.Logger = logger;
            this.Mapper = mapper;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var method = context.HttpContext.Request.Method;
                var path = context.HttpContext.Request.Path;
                var query = context.HttpContext.Request.QueryString.Value;
                var body = GetRequestBody(context.HttpContext.Request).Result;
                context.HttpContext.Request.Headers.TryGetValue("ClientVersion", out StringValues clientVersion);
                Logger.LogException(CurrentUserId, method, path + query, body, context.Exception, clientVersion.FirstOrDefault());
            }
            base.OnActionExecuted(context);
        }
        protected void LogClientVersion()
        {
            if(Request.Headers.TryGetValue("ClientVersion", out StringValues clientVersion))
            {
                Logger.LogClientVersion(CurrentUserId, clientVersion.FirstOrDefault());
            }
            else
            {
                Logger.LogClientVersion(CurrentUserId, "older");

            }
        }
        private static async Task<string> GetRequestBody(HttpRequest request)
        {
            var ms = new MemoryStream();
            request.Body.Position = 0;
            await request.Body.CopyToAsync(ms);
            using var reader = new StreamReader(ms);
            ms.Position = 0;
            return await reader.ReadToEndAsync();
        }
    }
}
