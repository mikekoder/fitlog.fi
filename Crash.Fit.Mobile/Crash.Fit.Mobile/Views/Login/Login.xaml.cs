using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crash.Fit.Mobile.Views.Login
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            WebView.Navigated += (sender, e) =>
            {
                if (e.Url.Contains("login-success"))
                {
                    /*
                    var cookies = cookieStore.GetCookies().ToArray();
                    using (var client = new ApiClient(cookies))
                    {
                        client.Test();
                    }*/
                }
            };
            WebView.Source = "https://fitlog.fi/api/users/external-login?provider=Facebook";
        }
    }
}