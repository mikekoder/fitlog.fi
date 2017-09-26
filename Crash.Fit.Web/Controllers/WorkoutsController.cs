using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Training;
using Crash.Fit.Api.Models.Training;
using Crash.Fit.Logging;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WorkoutsController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public WorkoutsController(ITrainingRepository trainingRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
        }

        [HttpGet("")]
        public IActionResult Search(DateTimeOffset start, DateTimeOffset? end)
        {
            var workouts = trainingRepository.SearchWorkouts(CurrentUserId, start, end ?? DateTimeOffset.Now).OrderByDescending(w => w.Time);

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse[]>(workouts);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            var workout = trainingRepository.GetWorkout(id);
            if(workout.UserId != CurrentUserId)
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse>(workout);
            return Ok(response);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]WorkoutRequest request)
        {
            CreateExercises(request.Sets);
            var workout = AutoMapper.Mapper.Map<WorkoutDetails>(request);
            workout.UserId = CurrentUserId;     
            trainingRepository.CreateWorkout(workout);

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse>(workout);
            return Ok(response);
        }

        [HttpPut("{id}")]
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

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse>(workout);
            return Ok(response);
        }

        [HttpDelete("{id}")]
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
            if(!sets.Any(s => s.ExerciseId == null))
            {
                return;
            }

            var exercises = new List<Exercise>();
            exercises.AddRange(trainingRepository.SearchUserExercises(CurrentUserId));
            foreach (var set in sets.Where(s => s.ExerciseId == null && !string.IsNullOrWhiteSpace(s.ExerciseName)))
            {
                var exercise = exercises.FirstOrDefault(e => e.Name.Equals(set.ExerciseName, StringComparison.CurrentCultureIgnoreCase));
                if (exercise != null)
                {
                    set.ExerciseId = exercise.Id;
                }
                else
                {
                    var newExercise = new ExerciseDetails
                    {
                        UserId = CurrentUserId,
                        Name = char.ToUpper(set.ExerciseName[0]) + set.ExerciseName.Substring(1).ToLower()
                    };
                    trainingRepository.CreateExercise(newExercise);
                    exercises.Add(newExercise);
                    set.ExerciseId = newExercise.Id;
                }
            }
        }
    }
}