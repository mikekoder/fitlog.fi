using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Api.Models.Nutrition;
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
                var nutrients = nutritionRepository.GetUserNutrients(CurrentUserId).OrderBy(n => n.UserOrder ?? int.MaxValue).ThenBy(n => n.DefaultOrder ?? int.MaxValue).ThenBy(n => n.Name);
                var response = AutoMapper.Mapper.Map<NutrientResponse[]>(nutrients);
                var goals = nutritionRepository.GetNutritionGoals(CurrentUserId);
                foreach (var nutrient in response)
                {
                    nutrient.Goals.Min = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days == Days.None)?.Min;
                    nutrient.Goals.Max = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days == Days.None)?.Max;

                    nutrient.Goals.MondayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Monday))?.Min;
                    nutrient.Goals.MondayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Monday))?.Max;

                    nutrient.Goals.TuesdayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Tuesday))?.Min;
                    nutrient.Goals.TuesdayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Tuesday))?.Max;

                    nutrient.Goals.WednesdayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Wednesday))?.Min;
                    nutrient.Goals.WednesdayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Wednesday))?.Max;

                    nutrient.Goals.ThursdayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Thursday))?.Min;
                    nutrient.Goals.ThursdayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Thursday))?.Max;

                    nutrient.Goals.FridayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Friday))?.Min;
                    nutrient.Goals.FridayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Friday))?.Max;

                    nutrient.Goals.SaturdayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Saturday))?.Min;
                    nutrient.Goals.SaturdayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Saturday))?.Max;

                    nutrient.Goals.SundayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Sunday))?.Min;
                    nutrient.Goals.SundayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.Sunday))?.Max;

                    nutrient.Goals.ExerciseDayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.ExerciseDay))?.Min;
                    nutrient.Goals.ExerciseDayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.ExerciseDay))?.Max;

                    nutrient.Goals.RestDayMin = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.RestDay))?.Min;
                    nutrient.Goals.RestDayMax = goals.FirstOrDefault(t => t.NutrientId == nutrient.Id && t.Days.HasFlag(Days.RestDay))?.Max;
                }

                return Ok(response);
            }

        }

        [HttpGet("goals")]
        public IActionResult ListGoals()
        {
            var response = new List<NutritionGoalsResponse>();
            var goals = nutritionRepository.GetNutritionGoals(CurrentUserId);
            var days = goals.Select(t => t.Days).Distinct();
            foreach (var day in days)
            {
                var responseDay = new NutritionGoalsResponse
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
                    NutrientValues = goals.Where(t => t.Days == day).Select(t => new NutritionGoalsResponse.NutrientValue
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
        [HttpPut("goals")]
        public IActionResult UpdateGoals([FromBody] NutritionGoalsRequest[] request)
        {
            var goals = new List<NutritionGoal>();
            foreach(var goalDay in request)
            {
                var days = Days.None;
                if (goalDay.Monday)
                {
                    days |= Days.Monday;
                }
                if (goalDay.Tuesday)
                {
                    days |= Days.Tuesday;
                }
                if (goalDay.Wednesday)
                {
                    days |= Days.Wednesday;
                }
                if (goalDay.Thursday)
                {
                    days |= Days.Thursday;
                }
                if (goalDay.Friday)
                {
                    days |= Days.Friday;
                }
                if (goalDay.Saturday)
                {
                    days |= Days.Saturday;
                }
                if (goalDay.Sunday)
                {
                    days |= Days.Sunday;
                }
                if (goalDay.ExerciseDay)
                {
                    days |= Days.ExerciseDay;
                }
                if (goalDay.RestDay)
                {
                    days |= Days.RestDay;
                }
                foreach (var nutrientValue in goalDay.NutrientValues)
                {
                    if (nutrientValue.Min.HasValue || nutrientValue.Max.HasValue)
                    {
                        goals.Add(new NutritionGoal
                        {
                            UserId = CurrentUserId,
                            NutrientId = nutrientValue.NutrientId,
                            Min = nutrientValue.Min,
                            Max = nutrientValue.Max,
                            Days = days
                        });
                    }
                }
            }

            nutritionRepository.SaveNutritionGoals(CurrentUserId, goals);
            return ListGoals();
        }

        [HttpPut("settings")]
        public IActionResult Settings([FromBody] NutrientSettingRequest[] request)
        {
            var settings = request.Select((s, index) => 
            {
                var setting = AutoMapper.Mapper.Map<NutrientSetting>(s);
                setting.UserId = CurrentUserId;
                setting.Order = index;
                return setting;
            });

            nutritionRepository.SaveNutrientSettings(CurrentUserId, settings);
            return List();
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