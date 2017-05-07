using Crash.Fit.Nutrition;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly INutritionRepository repo;
        public ExceptionFilter(INutritionRepository repo)
        {
            this.repo = repo;
        }
        public void OnException(ExceptionContext context)
        {
            var path = context.HttpContext.Request.Path;
            string body = null;
            var ms = new MemoryStream();
            context.HttpContext.Request.Body.Position = 0;
            context.HttpContext.Request.Body.CopyTo(ms);
            using(var reader = new StreamReader(ms))
            {
                ms.Position = 0;
                body = reader.ReadToEnd();
            }
        }
    }
}
