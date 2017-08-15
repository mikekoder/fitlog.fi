using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Mobile.Views.Main
{

    public class MainMenuItem
    {
        public MainMenuItem()
        {
            TargetType = typeof(MainMenuContainerDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}