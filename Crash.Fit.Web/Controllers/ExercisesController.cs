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
    public class ExercisesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        private readonly IMeasurementRepository measurementRepository;
        public ExercisesController(ITrainingRepository trainingRepository,IMeasurementRepository measurementRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
            this.measurementRepository = measurementRepository;
        }
        [HttpGet("")]
        public IActionResult List()
        {
            var exercises = trainingRepository.SearchUserExercises(CurrentUserId, DateTimeOffset.Now.AddMonths(-1)).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
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
            var exercise = trainingRepository.GetExercise(id);
            if(exercise == null || exercise.UserId != CurrentUserId)
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]ExerciseRequest request)
        {
            var exercise = AutoMapper.Mapper.Map<ExerciseDetails>(request);
            exercise.UserId = CurrentUserId;
            trainingRepository.CreateExercise(exercise);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]ExerciseRequest request)
        {
            var exercise = trainingRepository.GetExercise(id);
            if (exercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, exercise);
            trainingRepository.UpdateExercise(exercise);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var exercise = trainingRepository.GetExercise(id);
            if (exercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            trainingRepository.DeleteExercise(exercise);

            return Ok();
        }

        [HttpPut("{id}/onerepmax")]
        public IActionResult Update1RM(Guid id, [FromBody]decimal max)
        {
            var exercise = trainingRepository.GetExercise(id);
            if (exercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            
            var oneRepMap = new OneRepMax
            {
                ExerciseId = id,
                UserId = CurrentUserId,
                Time = DateTimeOffset.Now,
                Max = max
            };

            if (exercise.PercentageBW.HasValue)
            {
                var userWeight = measurementRepository.GetUserWeight(CurrentUserId);
                if (userWeight.HasValue)
                {
                    oneRepMap.MaxInclBW = oneRepMap.Max + (exercise.PercentageBW / 100m) * userWeight;
                }
            }
            else
            {
                oneRepMap.MaxInclBW = oneRepMap.MaxBW;
            }

            trainingRepository.SaveOneRepMaxs(new[] {oneRepMap});

            return Ok();
        }
        [HttpGet("history")]
        public IActionResult GetHistory(Guid exerciseId, DateTimeOffset start, DateTimeOffset end)
        {
            var exercises = trainingRepository.GetExerciseHistory(exerciseId, CurrentUserId, start, end);

            var response = AutoMapper.Mapper.Map<OneRepMaxResponse[]>(exercises);
            return Ok(response);
        }
    }
}