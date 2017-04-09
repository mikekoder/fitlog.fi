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
        [Route("")]
        public IActionResult Search(DateTimeOffset start, DateTimeOffset? end)
        {
            var workouts = trainingRepository.SearchWorkouts(CurrentUserId, start, end ?? DateTimeOffset.Now);

            var response = AutoMapper.Mapper.Map<WorkoutSummaryResponse[]>(workouts);
            return Ok(response);
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Details(Guid id)
        {
            var workout = trainingRepository.GetWorkout(id);
            if(workout.UserId != CurrentUserId)
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<WorkoutResponse>(workout);
            return Ok(response);
        }
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody]WorkoutRequest request)
        {
            CreateExercises(request.Sets);
            var workout = AutoMapper.Mapper.Map<WorkoutDetails>(request);
            workout.UserId = CurrentUserId;     
            trainingRepository.CreateWorkout(workout);

            var response = AutoMapper.Mapper.Map<WorkoutResponse>(workout);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, [FromBody]WorkoutRequest request)
        {
            var workout = trainingRepository.GetWorkout(id);
            if (workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            CreateExercises(request.Sets);
            AutoMapper.Mapper.Map(request, workout);
            trainingRepository.UpdateWorkout(workout);

            var response = AutoMapper.Mapper.Map<WorkoutResponse>(workout);
            return Ok(response);
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

        private void CreateExercises(IEnumerable<WorkoutSetRequest> sets)
        {
            var newExercises = sets.Where(s => s.ExerciseId == null && !string.IsNullOrWhiteSpace(s.ExerciseName))
                .Select(s => char.ToUpper(s.ExerciseName[0]) + s.ExerciseName.Substring(1).ToLower()).Distinct()
                .Select(s => new ExerciseDetails
                {
                    UserId = CurrentUserId,
                    Name = s
                });
            foreach(var exercise in newExercises)
            {
                trainingRepository.CreateExercise(exercise);
                foreach(var set in sets.Where(s => s.ExerciseId == null && exercise.Name.Equals(s.ExerciseName,StringComparison.CurrentCultureIgnoreCase)))
                {
                    set.ExerciseId = exercise.Id;
                }
            }
        }
    }
}