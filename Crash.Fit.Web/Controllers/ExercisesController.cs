using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Training;
using Crash.Fit.Web.Models.Training;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ExercisesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public ExercisesController(ITrainingRepository trainingRepository)
        {
            this.trainingRepository = trainingRepository;
        }
        [HttpGet]
        [Route("")]
        public IEnumerable<ExerciseMinimal> List()
        {
            var exercises = trainingRepository.SearchUserExercises(CurrentUserId).OrderBy(e => e.Name);
            return exercises;
        }
        [HttpGet]
        [Route("search")]
        public IEnumerable<ExerciseMinimal> Search(string name)
        {
            var exercises = trainingRepository.SearchExercises(name.Split(' '), CurrentUserId).OrderBy(e => e.Name);
            return exercises;
        }
        [HttpGet]
        [Route("{id}")]
        public ExerciseDetails Details(Guid id)
        {
            var exercise = trainingRepository.GetExercise(id);
            return exercise;
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
            return Ok(exercise);
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
            return Ok(exercise);
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