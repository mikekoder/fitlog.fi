using Crash.Fit.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Mobile
{
    public class Module : Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<IApiClient>().To<ApiClient>().WithConstructorArgument("baseUrl", "https://fitlog.fi");
            Bind<App>().ToSelf();
        }
    }
}
