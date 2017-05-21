using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Web.Models.Nutrition;
using Microsoft.Extensions.Logging;
using Crash.Fit.Logging;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NutrientsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public NutrientsController(INutritionRepository nutritionRepository, ILogRepository logger) : base(logger)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet("")]
        public IActionResult List()
        {
            if (CurrentUserId == Guid.Empty)
            {
                var nutrients = nutritionRepository.GetNutrients().OrderBy(n => n.DefaultOrder ?? int.MaxValue).ThenBy(n => n.Name);
                var response = AutoMapper.Mapper.Map<NutrientResponse[]>(nutrients);
                return Ok(response);
            }
            else
            {
                var nutrients = nutritionRepository.GetUserNutrients(CurrentUserId).OrderBy(n => n.Order ?? int.MaxValue).ThenBy(n => n.DefaultOrder ?? int.MaxValue).ThenBy(n => n.Name);
                var response = AutoMapper.Mapper.Map<NutrientResponse[]>(nutrients);
                var targets = nutritionRepository.GetNutrientTargets(CurrentUserId);
                foreach(var nutrient in response)
                {
                    nutrient.Targets.Min = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days == Days.None)?.Min;
                    nutrient.Targets.Max = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days == Days.None)?.Max;

                    nutrient.Targets.MondayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Monday))?.Min;
                    nutrient.Targets.MondayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Monday))?.Max;

                    nutrient.Targets.TuesdayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Tuesday))?.Min;
                    nutrient.Targets.TuesdayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Tuesday))?.Max;

                    nutrient.Targets.WednesdayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Wednesday))?.Min;
                    nutrient.Targets.WednesdayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Wednesday))?.Max;

                    nutrient.Targets.ThursdayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Thursday))?.Min;
                    nutrient.Targets.ThursdayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Thursday))?.Max;

                    nutrient.Targets.FridayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Friday))?.Min;
                    nutrient.Targets.FridayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Friday))?.Max;

                    nutrient.Targets.SaturdayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Saturday))?.Min;
                    nutrient.Targets.SaturdayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Saturday))?.Max;

                    nutrient.Targets.SundayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Sunday))?.Min;
                    nutrient.Targets.SundayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Sunday))?.Max;

                    nutrient.Targets.ExerciseDayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.ExerciseDay))?.Min;
                    nutrient.Targets.ExerciseDayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.ExerciseDay))?.Max;

                    nutrient.Targets.RestDayMin = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.RestDay))?.Min;
                    nutrient.Targets.RestDayMax = targets.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.RestDay))?.Max;
                }
                
                return Ok(response);
            }
           
        }

        [HttpGet("targets")]
        public IActionResult Targets()
        {
            var response = new List<NutrientTargetsResponse>();
            var targets = nutritionRepository.GetNutrientTargets(CurrentUserId);
            var days = targets.Select(t => t.Days).Distinct();
            foreach(var day in days)
            {
                var responseDay = new NutrientTargetsResponse
                {
                    Monday = day.HasFlag(Days.Monday),
                    Tuesday = day.HasFlag(Days.Tuesday),
                    Wednesday = day.HasFlag(Days.Wednesday),
                    Thursday = day.HasFlag(Days.Thursday),
                    Friday = day.HasFlag(Days.Friday),
                    Saturday = day.HasFlag(Days.Saturday),
                    Sunday = day.HasFlag(Days.Sunday),
                    ExerciseDay = day.HasFlag(Days.ExerciseDay),
                    RestDay = day.HasFlag(Days.RestDay),
                    NutrientTargets = targets.Where(t => t.Days == day).Select(t => new NutrientTargetsResponse.NutrientTarget
                    {
                        NutrientId = t.NutrientId,
                        Min = t.Min,
                        Max = t.Max
                    }).ToArray()
                };
                response.Add(responseDay);
            }
            return Ok(response);
        }
        /*
        [HttpGet]
        [Route("daily-intakes")]
        public IEnumerable<DailyIntake> DailyIntakes(Gender gender, DateTime dateOfBirth)
        {
            var age = DateTime.Now - dateOfBirth;
            return nutritionRepository.GetDailyIntakes(gender, age);
        }
        */
    }
}