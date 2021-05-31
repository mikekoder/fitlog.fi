using AutoMapper;
using Fitlog.Activities;
using Fitlog.Api.Models.Nutrition;
using Fitlog.Logging;
using Fitlog.Measurements;
using Fitlog.Nutrition;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Fitlog.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NutritionController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        private readonly IActivityRepository activityRepository;
        private readonly IMeasurementRepository measurementRepository;

        public NutritionController(INutritionRepository nutritionRepository, IActivityRepository activityRepository, IMeasurementRepository measurementRepository, ILogRepository logger, IMapper mapper) : base(logger, mapper)
        {
            this.nutritionRepository = nutritionRepository;
            this.activityRepository = activityRepository;
            this.measurementRepository = measurementRepository;
        }
        [HttpGet("nutrients")]
        [AllowAnonymous]
        public IActionResult Nutrients()
        {
            var nutrients = nutritionRepository.GetNutrients().OrderBy(n => n.DefaultOrder ?? int.MaxValue).ThenBy(n => n.Name);
            var response = Mapper.Map<NutrientResponse[]>(nutrients);
            return Ok(response);
        }
        
        [HttpGet("settings")]
        [AllowAnonymous]
        public IActionResult NutrientSettings()
        {
            if(CurrentUserId == Guid.Empty)
            {
                return Ok(Enumerable.Empty<NutrientSettingResponse>());
            }
            var nutrients = nutritionRepository.GetNutrientSettings(CurrentUserId);
            var response = Mapper.Map<NutrientSettingResponse[]>(nutrients);
            return Ok(response);
        }
        [HttpGet("goals/active")]
        public IActionResult ActiveGoal()
        {
            var goal = nutritionRepository.GetNutritionGoals(CurrentUserId).FirstOrDefault(g => g.Active);
            if(goal == null)
            {
                return Ok();
            }
            var response = Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(response);
        }
        
        [HttpGet("goals")]
        public IActionResult ListGoals()
        {
            var goals = nutritionRepository.GetNutritionGoals(CurrentUserId);
            var response = Mapper.Map<NutritionGoalResponse[]>(goals);

            return Ok(response);
        }
        [HttpGet("goals/{id}")]
        public IActionResult GetGoal(Guid id)
        {
            var goal = nutritionRepository.GetNutritionGoals(CurrentUserId).SingleOrDefault(g => g.Id == id);
            if(goal == null)
            {
                return NotFound();
            }
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            var response = Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(response);
        }
        [HttpPost("goals")]
        public IActionResult CreateGoal([FromBody] NutritionGoalRequest request)
        {
            var goal = Mapper.Map<NutritionGoalDetails>(request);
            goal.UserId = CurrentUserId;

            var goals = nutritionRepository.GetNutritionGoals(CurrentUserId);
            if (!goals.Any())
            {
                goal.Active = true;
            }
            nutritionRepository.CreateNutritionGoal(goal);

            var result = Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(result);
        }
        [HttpPut("goals/{id}")]
        public IActionResult UpdateGoal(Guid id, [FromBody] NutritionGoalRequest request)
        {
            var goal = nutritionRepository.GetNutritionGoal(id);
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            Mapper.Map(request, goal);
            nutritionRepository.UpdateNutritionGoal(goal);

            var result = Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(result);
        }
        [HttpPost("goals/{id}/activate")]
        public IActionResult ActivateGoal(Guid id)
        {
            var goal = nutritionRepository.GetNutritionGoal(id);
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            nutritionRepository.ActivateNutritionGoal(goal);
            return Ok();
        }
        [HttpDelete("goals/{id}")]
        public IActionResult DeleteGoal(Guid id)
        {
            var goal = nutritionRepository.GetNutritionGoal(id);
            if (goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            nutritionRepository.DeleteNutritionGoal(goal);
            return Ok();
        }
        [HttpPut("settings")]
        public IActionResult UpdateSettings([FromBody] NutrientSettingRequest[] request)
        {
            var settings = request.Select((s, index) => 
            {
                var setting = Mapper.Map<NutrientSetting>(s);
                setting.UserId = CurrentUserId;
                setting.Order = index;
                return setting;
            });

            nutritionRepository.SaveNutrientSettings(settings);

            var response = Mapper.Map<NutrientSettingResponse[]>(settings);
            return Ok(response);
        }

        [HttpGet("nutrients/history")]
        public IActionResult GetNutrientHistory(DateTimeOffset start, DateTimeOffset end)
        {
            var nutrients = nutritionRepository.GetDailyNutrients(CurrentUserId, start, end);
            var activityPresets = activityRepository.GetActivityPresets(CurrentUserId);
            var measurements = measurementRepository.GetMeasurementHistory(Constants.Measurements.RmrId, CurrentUserId, DateTimeOffset.MinValue, end);
            var energyExpenditures = activityRepository.GetEnergyExpenditures(CurrentUserId, start, end);
            var days = activityRepository.GetActivityPresetsForDays(CurrentUserId, start, end);

            var response = Mapper.Map<NutrientHistoryResponse[]>(nutrients);

            foreach(var day in response)
            {
                NutritionUtils.AppendComputedNutrients(day.Nutrients);
                var energyExpenditure = 0m;
                var rmr = measurements
                    .Where(m => m.MeasureId == Constants.Measurements.RmrId && m.Time.Date <= day.Date)
                    .OrderByDescending(m => m.Time)
                    .FirstOrDefault()?.Value;

                if (rmr.HasValue)
                {
                    energyExpenditure += rmr.Value;
                    ActivityPreset preset = null;
                    if (days.TryGetValue(day.Date, out Guid presetId))
                    {
                        preset = activityPresets.FirstOrDefault(p => p.Id == presetId);
                    }
                    else
                    {
                        switch (day.Date.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                preset = activityPresets.FirstOrDefault(p => p.Monday);
                                break;
                            case DayOfWeek.Tuesday:
                                preset = activityPresets.FirstOrDefault(p => p.Tuesday);
                                break;
                            case DayOfWeek.Wednesday:
                                preset = activityPresets.FirstOrDefault(p => p.Wednesday);
                                break;
                            case DayOfWeek.Thursday:
                                preset = activityPresets.FirstOrDefault(p => p.Thursday);
                                break;
                            case DayOfWeek.Friday:
                                preset = activityPresets.FirstOrDefault(p => p.Friday);
                                break;
                            case DayOfWeek.Saturday:
                                preset = activityPresets.FirstOrDefault(p => p.Saturday);
                                break;
                            case DayOfWeek.Sunday:
                                preset = activityPresets.FirstOrDefault(p => p.Sunday);
                                break;
                        }
                    }
                    if (preset != null)
                    {
                        energyExpenditure += (preset.Factor - 1) * rmr.Value;
                    }
                }
                foreach(var expenditure in energyExpenditures.Where(e => e.Time.Date == day.Date.Date))
                {
                    energyExpenditure += expenditure.EnergyKcal;
                }

                day.EnergyExpenditure = energyExpenditure;
                day.Nutrients[Constants.Nutrition.EnergyDifferenceId] = day.Nutrients[Constants.Nutrition.EnergyKcalId] - energyExpenditure;
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