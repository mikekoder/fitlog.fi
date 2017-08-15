using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Crash.Fit.Mobile
{
    public class ViewHelpers
    {
        public static void CenterLoader(ActivityIndicator loader)
        {
            AbsoluteLayout.SetLayoutFlags(loader, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(loader, new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        }
    }
}
