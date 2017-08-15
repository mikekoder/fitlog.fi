using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using Android.Webkit;
using Crash.Fit.MobileServices;

namespace Crash.Fit.Mobile.Droid
{
    public class CookieStore : ICookieStore
    {
        public IEnumerable<Cookie> GetCookies(string url)
        {
            var allCookiesForUrl = CookieManager.Instance.GetCookie(url);
            if (string.IsNullOrWhiteSpace(allCookiesForUrl))
            {
                yield break;
            }

            var pairs = allCookiesForUrl.Split(' ');
            foreach (var pair in pairs)
            {
                var parts = pair.Split(new[] { '=' }, 2);
                yield return new Cookie(parts[0], parts.Length > 1 ? parts[1].TrimEnd(';') : "", "/", new Uri(url).DnsSafeHost);
            }
        }
    }
}