using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Api
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
    public class ApiResult<T> : ApiResult
    {
        
        public T Data { get; set; }
    }
}
