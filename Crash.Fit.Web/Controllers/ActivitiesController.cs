using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Training;
using Microsoft.AspNetCore.Mvc.Filters;
using Crash.Fit.Logging;
using Crash.Fit.Api.Models.Training;
using Crash.Fit.Measurements;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ActivitiesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        private readonly IMeasurementRepository measurementRepository;
        public ActivitiesController(ITrainingRepository trainingRepository, IMeasurementRepository measurementRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
            this.measurementRepository = measurementRepository;
        }
        [HttpGet("")]
        public IActionResult List()
        {
            var activities = trainingRepository.GetActivities().OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ActivityResponse[]>(activities);
            return Ok(response);
        }
        [HttpGet("search")]
        public IActionResult Search(string name)
        {
            var exercises = trainingRepository.SearchExercises(name.Split(' '), CurrentUserId).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            var activity = trainingRepository.GetActivity(id);
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
            trainingRepository.CreateActivity(activity);

            var response = AutoMapper.Mapper.Map<ActivityResponse>(activity);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]ActivityRequest request)
        {
            var activity = trainingRepository.GetActivity(id);
            AutoMapper.Mapper.Map(request, activity);
            trainingRepository.UpdateActivity(activity);

            var response = AutoMapper.Mapper.Map<ActivityResponse>(activity);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var activity = trainingRepository.GetActivity(id);
            trainingRepository.DeleteActivity(activity);

            return Ok();
        }

        [HttpGet("energyexpenditures")]
        public IActionResult GetEnergyExpenditures(DateTimeOffset start, DateTimeOffset? end)
        {
            var expenditures = trainingRepository.GetEnergyExpenditures(CurrentUserId, start, end ?? DateTimeOffset.Now).OrderByDescending(e => e.Time);
            var response = AutoMapper.Mapper.Map<EnergyExpenditureResponse[]>(expenditures);
            return Ok(response);
        }
        [HttpPost("energyexpenditures")]
        public IActionResult CreateEnergyExpenditure([FromBody]EnergyExpenditureRequest request)
        {
            var expenditure = AutoMapper.Mapper.Map<EnergyExpenditure>(request);
            expenditure.UserId = CurrentUserId;
            CalculateEnergy(expenditure);
            trainingRepository.CreateEnergyExpenditure(expenditure);

            var response = AutoMapper.Mapper.Map<EnergyExpenditureResponse>(expenditure);
            return Ok(response);
        }
        [HttpPut("energyexpenditures/{id}")]
        public IActionResult UpdateEnergyExpenditure(Guid id, [FromBody]EnergyExpenditureRequest request)
        {
            var expenditure = trainingRepository.GetEnergyExpenditure(id);
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
            trainingRepository.UpdateEnergyExpenditure(expenditure);

            var response = AutoMapper.Mapper.Map<EnergyExpenditureResponse>(expenditure);
            return Ok(response);
        }
        [HttpDelete("energyexpenditures/{id}")]
        public IActionResult DeleteEnergyExpenditure(Guid id)
        {
            var expenditure = trainingRepository.GetEnergyExpenditure(id);
            if (expenditure == null)
            {
                return NotFound();
            }
            if (expenditure.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            
            trainingRepository.DeleteEnergyExpenditure(expenditure);

            return Ok();
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
                var activity = trainingRepository.GetActivity(expenditure.ActivityId.Value);
                expenditure.EnergyKcal = activity.EnergyExpenditure * userWeight.Value * (decimal)expenditure.Duration.Value.TotalMinutes;
            }
        }
    }
}