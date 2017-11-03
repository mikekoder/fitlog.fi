using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Training;
using Crash.Fit.Api.Models.Training;
using Crash.Fit.Logging;
using Crash.Fit.Measurements;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class WorkoutsController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        private readonly IMeasurementRepository measurementRepository;
        public WorkoutsController(ITrainingRepository trainingRepository, IMeasurementRepository measurementRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
            this.measurementRepository = measurementRepository;
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
            if (workout.UserId != CurrentUserId)
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse>(workout);
            return Ok(response);
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]WorkoutRequest request)
        {
            var exercises = CreateExercises(request.Sets);
            var workout = AutoMapper.Mapper.Map<WorkoutDetails>(request);
            workout.UserId = CurrentUserId;
            var maxs = Calculate1RMs(workout, exercises);
            trainingRepository.SaveOneRepMaxs(maxs);
            CalculateLoads(workout.Sets, exercises);
            trainingRepository.CreateWorkout(workout);

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse>(workout);
            return Ok(response);
        }

        

        [HttpPost("start")]
        public IActionResult Start([FromBody]WorkoutStartModel request)
        {
            var workout = new WorkoutDetails
            {
                UserId = CurrentUserId,
                Time = request.Time
            };
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
            var exercises = CreateExercises(request.Sets);
            AutoMapper.Mapper.Map(request, workout);
            var maxs = Calculate1RMs(workout, exercises);
            trainingRepository.SaveOneRepMaxs(maxs);
            CalculateLoads(workout.Sets, exercises);
            trainingRepository.UpdateWorkout(workout);

            var response = AutoMapper.Mapper.Map<WorkoutDetailsResponse>(workout);
            return Ok(response);
        }
        [HttpPut("{id}/time")]
        public IActionResult UpdateTime(Guid id, DateTimeOffset time)
        {
            // TODO: optimate
            var workout = trainingRepository.GetWorkout(id);
            if(workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            workout.Time = time;
            trainingRepository.UpdateWorkout(workout);

            return Ok();
        }
        [HttpPost("{id}")]
        public IActionResult AddSet(Guid id, [FromBody]WorkoutSetRequest request)
        {
            var workout = trainingRepository.GetWorkout(id);
            if (workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            CreateExercises(new[] { request });
            var set = AutoMapper.Mapper.Map<WorkoutSet>(request);
            workout.Sets = workout.Sets.Union(new[] { set }).ToArray();
            trainingRepository.UpdateWorkout(workout);

            var result = AutoMapper.Mapper.Map<WorkoutSetResponse>(set);
            return Ok(result);
        }
        [HttpPut("{id}/sets/{setId}")]
        public IActionResult UpdateSet(Guid id, Guid setId, [FromBody]WorkoutSetRequest request)
        {
            var workout = trainingRepository.GetWorkout(id);
            if (workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            var set = workout.Sets.FirstOrDefault(s => s.Id == setId);
            CreateExercises(new[] { request });
            set.ExerciseId = request.ExerciseId.Value;
            set.ExerciseName = request.ExerciseName;
            set.Reps = request.Reps;
            set.Weights = request.Weights;
            trainingRepository.UpdateWorkout(workout);
            return Ok();
        }
        [HttpDelete("{id}/sets/{setId}")]
        public IActionResult DeleteSet(Guid id, Guid setId)
        {
            var workout = trainingRepository.GetWorkout(id);
            if (workout.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            workout.Sets = workout.Sets.Where(s => s.Id != setId).ToArray();
            trainingRepository.UpdateWorkout(workout);
            return Ok();
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
        private decimal? GetUserWeight()
        {
            return measurementRepository.GetMeasures(CurrentUserId).FirstOrDefault(m => m.Id == Constants.Measurements.WeightId)?.LatestValue;
        }
        private IEnumerable<Exercise> CreateExercises(IEnumerable<WorkoutSetRequest> sets)
        {
            var exerciseIds = sets.Where(s => s.ExerciseId.HasValue).Select(s => s.ExerciseId.Value);
            var exercises = new List<Exercise>();
            exercises.AddRange(trainingRepository.GetExercises(exerciseIds));
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
            return exercises;
        }
        private IEnumerable<OneRepMax> Calculate1RMs(WorkoutDetails workout, IEnumerable<Exercise> exercises)
        {
            var userWeight = GetUserWeight();
            var maxs = new List<OneRepMax>();
            foreach (var set in workout.Sets.Where(s => s.Reps > 0 && s.Reps <= 10 && s.Weights > 0))
            {
                var exercise = exercises.FirstOrDefault(e => e.Id == set.ExerciseId);
                maxs.Add(new OneRepMax
                {
                    UserId = CurrentUserId,
                    ExerciseId = set.ExerciseId,
                    Time = workout.Time,
                    Max = TrainingUtils.Calculate1RM(set.Reps, set.Weights),
                    MaxBW = (userWeight.HasValue && exercise.BodyWeightPercentage.HasValue) ? 
                        TrainingUtils.Calculate1RM(set.Reps, set.Weights + userWeight.Value * (exercise.BodyWeightPercentage.Value/100)) : 
                        null as decimal?,
                });
            }
            return maxs.GroupBy(m => m.ExerciseId).Select(m => m.OrderByDescending(m2 => m2.Max).First());
        }
        private void CalculateLoads(IEnumerable<WorkoutSet> sets, IEnumerable<Exercise> exercises)
        {
            var userWeight = GetUserWeight();
            var maxs = trainingRepository.GetOneRepMaxs(CurrentUserId, DateTimeOffset.Now.AddDays(-30));
            foreach(var set in sets.Where(s => s.Weights > 0))
            {
                var exercise = exercises.FirstOrDefault(e => e.Id == set.ExerciseId);
                var max = maxs.FirstOrDefault(m => m.ExerciseId == set.ExerciseId);
                if(max != null)
                {
                    set.Load = set.Weights / max.Max * 100;
                    if (userWeight.HasValue && exercise.BodyWeightPercentage.HasValue && max.MaxBW.HasValue)
                    {
                        set.LoadBW = (set.Weights + userWeight.Value * (exercise.BodyWeightPercentage.Value / 100)) / max.MaxBW * 100;
                    }
                }
                
            }
        }
    }
}