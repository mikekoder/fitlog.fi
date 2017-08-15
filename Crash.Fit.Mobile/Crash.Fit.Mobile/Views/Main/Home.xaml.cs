using Crash.Fit.Api;
using Crash.Fit.Api.Models.Nutrition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Crash.Fit.Mobile.Views.Main
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        public Home()
        {
            InitializeComponent();
            ViewHelpers.CenterLoader(Loader);

            CreateMealButton.Clicked += (sender, e) => 
            {

            };

            MealList.ItemsSource = new[] 
            {
                new MealSummaryViewModel{Time = "13:30" },
                new MealSummaryViewModel{Time = "11:30" },
                new MealSummaryViewModel{Time = "8:00" },
            };
            Content.IsVisible = false;
            Loader.IsVisible = true;
        }
        private async Task LoadData()
        {
            Content.IsVisible = false;
            Loader.IsVisible = true;

            //var client = new ApiClient("https://fitlog.fi");


            Loader.IsVisible = false;
            Content.IsVisible = true;
            
        }
    }
}