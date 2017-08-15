using Crash.Fit.MobileServices;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Crash.Fit.Mobile
{
    public partial class App : Application
    {
        private readonly IReadOnlyKernel kernel;
        //private readonly IContainer container;
        public App(IReadOnlyKernel kernel)
        {
            this.kernel = kernel;
            InitializeComponent();

            var cookies = kernel.Get<ICookieStore>().GetCookies("https://fitlog.fi");
            //MainPage = new Crash.Fit.Mobile.MainPage();
            MainPage = new Crash.Fit.Mobile.Views.Main.MainMenuContainer();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
