using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.MobileServices
{
    public interface ICookieStore
    {
        IEnumerable<Cookie> GetCookies(string url);
    }
}
