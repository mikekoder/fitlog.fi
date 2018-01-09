using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Training;
using Microsoft.AspNetCore.Mvc.Filters;
using Crash.Fit.Logging;
using Crash.Fit.Measurements;
using Crash.Fit.Activities;
using Crash.Fit.Api.Models.Activities;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ActivitiesController : ApiControllerBase
    {
        private readonly IActivityRepository activityRepository;
        private readonly IMeasurementRepository measurementRepository;
        public ActivitiesController(IActivityRepository activityRepository, IMeasurementRepository measurementRepository, ILogRepository logger) : base(logger)
        {
            this.activityRepository = activityRepository;
            this.measurementRepository = measurementRepository;
        }
        [HttpGet("")]
        public IActionResult List()
        {
            var activities = activityRepository.GetActivities().OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ActivityResponse[]>(activities);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            var activity = activityRepository.GetActivity(id);
            if(activity == null)
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<ActivityResponse>(activity);
            return Ok(response);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]ActivityRequest request)
        {
            var activity = AutoMapper.Mapper.Map<Activity>(request);
            activityRepository.CreateActivity(activity);

            var response = AutoMapper.Mapper.Map<ActivityResponse>(activity);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]ActivityRequest request)
        {
            var activity = activityRepository.GetActivity(id);
            AutoMapper.Mapper.Map(request, activity);
            activityRepository.UpdateActivity(activity);

            var response = AutoMapper.Mapper.Map<ActivityResponse>(activity);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var activity = activityRepository.GetActivity(id);
            activityRepository.DeleteActivity(activity);

            return Ok();
        }

        [HttpGet("energyexpenditures")]
        public IActionResult GetEnergyExpenditures(DateTimeOffset start, DateTimeOffset? end)
        {
            var expenditures = activityRepository.GetEnergyExpenditures(CurrentUserId, start, end ?? DateTimeOffset.Now).OrderByDescending(e => e.Time);
            var response = AutoMapper.Mapper.Map<EnergyExpenditureResponse[]>(expenditures);
            return Ok(response);
        }
        [HttpPost("energyexpenditures")]
        public IActionResult CreateEnergyExpenditure([FromBody]EnergyExpenditureRequest request)
        {
            var expenditure = AutoMapper.Mapper.Map<EnergyExpenditure>(request);
            expenditure.UserId = CurrentUserId;
            CalculateEnergy(expenditure);
            activityRepository.CreateEnergyExpenditure(expenditure);

            var response = AutoMapper.Mapper.Map<EnergyExpenditureResponse>(expenditure);
            return Ok(response);
        }
        [HttpPut("energyexpenditures/{id}")]
        public IActionResult UpdateEnergyExpenditure(Guid id, [FromBody]EnergyExpenditureRequest request)
        {
            var expenditure = activityRepository.GetEnergyExpenditure(id);
            if(expenditure == null)
            {
                return NotFound();
            }
            if (expenditure.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, expenditure);
            CalculateEnergy(expenditure);
            activityRepository.UpdateEnergyExpenditure(expenditure);

            var response = AutoMapper.Mapper.Map<EnergyExpenditureResponse>(expenditure);
            return Ok(response);
        }
        [HttpDelete("energyexpenditures/{id}")]
        public IActionResult DeleteEnergyExpenditure(Guid id)
        {
            var expenditure = activityRepository.GetEnergyExpenditure(id);
            if (expenditure == null)
            {
                return NotFound();
            }
            if (expenditure.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            
            activityRepository.DeleteEnergyExpenditure(expenditure);

            return Ok();
        }

        [HttpGet("presets")]
        public IActionResult GetActivityPresets()
        {
            var presets = activityRepository.GetActivityPresets(CurrentUserId);

            var response = AutoMapper.Mapper.Map<ActivityPresetResponse[]>(presets);
            return Ok(response);
        }
        [HttpPut("presets")]
        public IActionResult UpdateActivityPresets([FromBody]ActivityPresetRequest[] request)
        {
            var presets = AutoMapper.Mapper.Map<ActivityPreset[]>(request);
            foreach(var preset in presets)
            {
                preset.UserId = CurrentUserId;
                var inactivityHours = 24 - preset.Sleep - preset.LightActivity - preset.ModerateActivity - preset.HeavyActivity;
                preset.Factor = (Constants.Activities.SleepFactor * preset.Sleep + 
                    Constants.Activities.InactivityFactor * inactivityHours + 
                    Constants.Activities.LightActivityFactor * preset.LightActivity + 
                    Constants.Activities.ModerateActivityFactor * preset.ModerateActivity + 
                    Constants.Activities.HeavyActivityFactor * preset.HeavyActivity) / 24;

            }
            activityRepository.SaveActivityPresets(presets);
            return GetActivityPresets();
        }

        private void CalculateEnergy(EnergyExpenditure expenditure)
        {
            var userWeight = measurementRepository.GetUserWeight(CurrentUserId);
            if (!userWeight.HasValue)
            {
                return;
            }
            if (expenditure.ActivityId.HasValue && expenditure.Duration.HasValue)
            {
                var activity = activityRepository.GetActivity(expenditure.ActivityId.Value);
                expenditure.EnergyKcal = activity.EnergyExpenditure * userWeight.Value * (decimal)expenditure.Duration.Value.TotalMinutes;
            }
        }
    }
}