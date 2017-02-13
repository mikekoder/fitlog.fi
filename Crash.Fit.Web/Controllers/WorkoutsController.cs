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
    public class WorkoutsController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public WorkoutsController(ITrainingRepository trainingRepository)
        {
            this.trainingRepository = trainingRepository;
        }

        [HttpGet]
        [Route("search")]
        public IEnumerable<WorkoutMinimal> Search(DateTimeOffset start, DateTimeOffset? end)
        {
            var workouts = trainingRepository.SearchWorkouts(CurrentUserId, start, end ?? DateTimeOffset.Now);
            return workouts;
        }
        [HttpGet]
        [Route("{id}")]
        public WorkoutDetails Details(Guid id)
        {
            var workout = trainingRepository.GetWorkout(id);
            return workout;
        }
        [HttpPost]
        [Route("")]
        public WorkoutDetails Create(WorkoutRequest request)
        {
            var workout = AutoMapper.Mapper.Map<WorkoutDetails>(request);
            trainingRepository.CreateWorkout(workout);
            return workout;
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, WorkoutRequest request)
        {
            var workout = trainingRepository.GetWorkout(id);
            if (workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, workout);
            trainingRepository.UpdateWorkout(workout);
            return Ok(workout);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var workout = trainingRepository.GetWorkout(id);
            if (workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            trainingRepository.DeleteWorkout(workout);
            return Ok();
        }
    }
}