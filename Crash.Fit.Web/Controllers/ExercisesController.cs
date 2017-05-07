using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Training;
using Crash.Fit.Web.Models.Training;
using Microsoft.AspNetCore.Mvc.Filters;
using Crash.Fit.Logging;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ExercisesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public ExercisesController(ITrainingRepository trainingRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
        }
        [HttpGet]
        [Route("")]
        public IActionResult List()
        {
            var exercises = trainingRepository.SearchUserExercises(CurrentUserId).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseSummaryResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet]
        [Route("search")]
        public IActionResult Search(string name)
        {
            var exercises = trainingRepository.SearchExercises(name.Split(' '), CurrentUserId).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet]
        [Route("{id}")]
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
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody]ExerciseRequest request)
        {
            var exercise = AutoMapper.Mapper.Map<ExerciseDetails>(request);
            exercise.UserId = CurrentUserId;
            if (!trainingRepository.CreateExercise(exercise))
            {
                return BadRequest();
            }

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, [FromBody]ExerciseRequest request)
        {
            var exercise = trainingRepository.GetExercise(id);
            if (exercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, exercise);
            if(!trainingRepository.UpdateExercise(exercise))
            {
                return BadRequest();
            }

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id}")]
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
    }
}