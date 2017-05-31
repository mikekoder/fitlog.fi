using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Web.Models.Nutrition
{
    public class NutrientResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Unit { get; set; }

        public string FineliId { get; set; }
        public string FineliClass { get; set; }
        public string FineliGroup { get; set; }

        public int Precision { get; set; }

        public int? DefaultOrder { get; set; }
        public bool DefaultHideSummary { get; set; }
        public bool DefaultHideDetails { get; set; }

        public int? UserOrder { get; set; }
        public bool? UserHideSummary { get; set; }
        public bool? UserHideDetails { get; set; }

        public bool HideSummary { get; set; }
        public bool HideDetails { get; set; }

        public NutrientTargets Targets { get; set; }

        public NutrientResponse()
        {
            Targets = new NutrientTargets();
        }

        public class NutrientTargets
        {
            public decimal? Min { get; set; }
            public decimal? Max { get; set; }

            public decimal? MondayMin { get; set; }
            public decimal? MondayMax { get; set; }
            public decimal? TuesdayMin { get; set; }
            public decimal? TuesdayMax { get; set; }
            public decimal? WednesdayMin { get; set; }
            public decimal? WednesdayMax { get; set; }
            public decimal? ThursdayMin { get; set; }
            public decimal? ThursdayMax { get; set; }
            public decimal? FridayMin { get; set; }
            public decimal? FridayMax { get; set; }
            public decimal? SaturdayMin { get; set; }
            public decimal? SaturdayMax { get; set; }
            public decimal? SundayMin { get; set; }
            public decimal? SundayMax { get; set; }
            public decimal? ExerciseDayMin { get; set; }
            public decimal? ExerciseDayMax { get; set; }
            public decimal? RestDayMin { get; set; }
            public decimal? RestDayMax { get; set; }
        }
    }
}
