using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crash.Fit.Mobile.Views.Foods
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Foods : TabbedPage
    {
        public Foods()
        {
            InitializeComponent();
            Children.Add(new MyFoods());
            Children.Add(new AllFoods());
        }
    }
}