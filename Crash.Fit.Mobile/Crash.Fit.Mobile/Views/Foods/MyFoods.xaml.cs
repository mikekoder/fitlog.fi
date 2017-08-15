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
    public partial class MyFoods : ContentPage
    {
        public MyFoods()
        {
            InitializeComponent();
            Title = "Omat ruoka-aineet";
            Results.ItemsSource = new[] { "Ruoka 1", "Ruoka 2", "Ruoka 3" };
        }

        private void Search(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new FoodEditor());
        }
    }
}