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
        [Route("search")]
        public IEnumerable<ExerciseMinimal> Search(string name)
        {
            var exercises = trainingRepository.SearchExercises(name);
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
        public ExerciseDetails Create(ExerciseRequest request)
        {
            var exercise = AutoMapper.Mapper.Map<ExerciseDetails>(request);
            trainingRepository.CreateExercise(exercise);
            return exercise;
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, ExerciseRequest request)
        {
            var exercise = trainingRepository.GetExercise(id);
            if (exercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, exercise);
            trainingRepository.UpdateExercise(exercise);
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