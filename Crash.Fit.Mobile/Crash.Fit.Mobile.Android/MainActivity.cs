using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Ninject;
using Ninject.Modules;
using Crash.Fit.MobileServices;

namespace Crash.Fit.Mobile.Droid
{
    [Activity(Label = "Crash.Fit.Mobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            var ninjectConfig = new KernelConfiguration(new Module());
            ninjectConfig.Bind<ICookieStore>().To<CookieStore>();
            var kernel = ninjectConfig.BuildReadonlyKernel();

            LoadApplication(kernel.Get<App>());
        }
    }
}

