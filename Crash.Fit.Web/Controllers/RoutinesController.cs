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
    public class RoutinesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public RoutinesController(ITrainingRepository trainingRepository)
        {
            this.trainingRepository = trainingRepository;
        }

        [HttpGet]
        [Route("")]
        public IActionResult List()
        {
            var routines = trainingRepository.SearchRoutines(CurrentUserId);

            var response = AutoMapper.Mapper.Map<RoutineSummaryResponse[]>(routines);
            return Ok(response);
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Details(Guid id)
        {
            var routine = trainingRepository.GetRoutine(id);
            if(routine == null || routine.UserId != CurrentUserId)
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<RoutineDetailsResponse>(routine);
            return Ok(response);
        }
        [HttpPost]
        [Route("")]
        public IActionResult Create([FromBody]RoutineRequest request)
        {
            CreateExercises(request.Workouts.SelectMany(w => w.Exercises));
            var routine = AutoMapper.Mapper.Map<RoutineDetails>(request);
            routine.UserId = CurrentUserId;
            trainingRepository.CreateRoutine(routine);

            var response = AutoMapper.Mapper.Map<RoutineDetailsResponse>(routine);
            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, [FromBody]RoutineRequest request)
        {
            var routine = trainingRepository.GetRoutine(id);
            if (routine.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            CreateExercises(request.Workouts.SelectMany(w => w.Exercises));
            AutoMapper.Mapper.Map(request, routine);
            trainingRepository.UpdateRoutine(routine);

            var response = AutoMapper.Mapper.Map<RoutineDetailsResponse>(routine);
            return Ok(response);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(Guid id)
        {
            var routine = trainingRepository.GetRoutine(id);
            if (routine.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            trainingRepository.DeleteRoutine(routine);

            return Ok();
        }
        private void CreateExercises(IEnumerable<RoutineExerciseRequest> sets)
        {
            if (!sets.Any(s => s.ExerciseId == null))
            {
                return;
            }

            var exercises = trainingRepository.SearchUserExercises(CurrentUserId).ToList();
            foreach (var set in sets.Where(s => s.ExerciseId == null && !string.IsNullOrWhiteSpace(s.ExerciseName)))
            {
                var exercise = exercises.FirstOrDefault(e => e.Name.Equals(set.ExerciseName, StringComparison.CurrentCultureIgnoreCase));
                if (exercise == null)
                {
                    exercise = new ExerciseDetails
                    {
                        UserId = CurrentUserId,
                        Name = char.ToUpper(set.ExerciseName[0]) + set.ExerciseName.Substring(1).ToLower()
                    };
                    trainingRepository.CreateExercise((ExerciseDetails)exercise);
                    exercises.Add(exercise);
                }
                set.ExerciseId = exercise.Id;
            }
        }
    }
}