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
using Microsoft.AspNetCore.Authorization;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ExercisesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        private readonly IMeasurementRepository measurementRepository;
        public ExercisesController(ITrainingRepository trainingRepository, IMeasurementRepository measurementRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
            this.measurementRepository = measurementRepository;
        }
        [HttpGet("equipment")]
        public IActionResult ListEquipment()
        {
            var equipment = trainingRepository.GetEquipment().OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<EquipmentResponse[]>(equipment);
            return Ok(response);
        }
        [HttpGet("")]
        public IActionResult List()
        {
            var exercises = trainingRepository.SearchUserExercises(CurrentUserId, DateTimeOffset.Now.AddMonths(-1)).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet("search")]
        public IActionResult Search(string name, Guid? muscleGroupId, Guid? equipmentId)
        {
            var exercises = trainingRepository.SearchExercises(name?.Split(' '), muscleGroupId, equipmentId, CurrentUserId, DateTimeOffset.Now.AddMonths(-1))
                .OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
            return Ok(response);
        }
        // TODO: can be deleted when all clients are updated
        [HttpGet("list/{ids}")]
        public IActionResult List_Old([ModelBinder(typeof(CommaSeparatedValueBinder<Guid>))]Guid[] ids)
        {
            var exercises = trainingRepository.GetExercises(ids.Distinct(), CurrentUserId, DateTimeOffset.Now.AddMonths(-1)).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet("list")]
        public IActionResult List([ModelBinder(typeof(CommaSeparatedValueBinder<Guid>))]Guid[] ids)
        {
            var exercises = trainingRepository.GetExercises(ids.Distinct(), CurrentUserId, DateTimeOffset.Now.AddMonths(-1)).OrderBy(e => e.Name);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet("latest")]
        public IActionResult ListLatest()
        {
            var exercises = trainingRepository.ListLatestExercises(CurrentUserId, DateTimeOffset.Now.AddMonths(-1));

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet("most-used")]
        public IActionResult ListMostUsed()
        {
            var exercises = trainingRepository.ListMostUsedExercises(CurrentUserId, DateTimeOffset.Now.AddMonths(-1));

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse[]>(exercises);
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            var exercise = trainingRepository.GetExercise(id, CurrentUserId, DateTimeOffset.Now.AddMonths(-1));
            if (exercise == null || (exercise.UserId.HasValue && exercise.UserId != CurrentUserId))
            {
                return NotFound();
            }

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            foreach (var image in response.Images)
            {
                image.Url = Url.Action("Image", "Exercises", new { id, imageId = image.Id }, Request.Scheme);
            }
            return Ok(response);
        }
        [HttpGet("{id}/images/{imageId}")]
        [AllowAnonymous]
        public IActionResult Image(Guid id, Guid imageId)
        {
            var image = trainingRepository.GetExerciseImage(id, imageId);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Data, "image/svg+xml");
        }
        [HttpPost("")]
        public IActionResult Create([FromBody]ExerciseRequest request)
        {
            var exercise = AutoMapper.Mapper.Map<ExerciseDetails>(request);
            exercise.UserId = CurrentUserId;
            if (exercise.Targets != null && exercise.SecondaryTargets != null)
            {
                exercise.SecondaryTargets = exercise.SecondaryTargets.Except(exercise.Targets).ToArray();
            }
            trainingRepository.CreateExercise(exercise);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]ExerciseRequest request)
        {
            var exercise = trainingRepository.GetExercise(id, CurrentUserId, DateTimeOffset.MinValue);
            if (exercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            AutoMapper.Mapper.Map(request, exercise);
            if (exercise.Targets != null && exercise.SecondaryTargets != null)
            {
                exercise.SecondaryTargets = exercise.SecondaryTargets.Except(exercise.Targets).ToArray();
            }
            trainingRepository.UpdateExercise(exercise);

            var response = AutoMapper.Mapper.Map<ExerciseDetailsResponse>(exercise);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var exercise = trainingRepository.GetExercise(id, CurrentUserId, DateTimeOffset.MinValue);
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
            var exercise = trainingRepository.GetExercise(id, CurrentUserId, DateTimeOffset.MinValue);
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

            trainingRepository.SaveOneRepMaxs(new[] { oneRepMap });

            return Ok();
        }
        [HttpGet("history")]
        public IActionResult GetHistory(Guid exerciseId, DateTimeOffset start, DateTimeOffset end)
        {
            var oneRepMaxHistory = trainingRepository.GetOneRepMaxHistory(exerciseId, CurrentUserId, start, end);
            var volumeHistory = trainingRepository.GetExerciseVolumeHistory(exerciseId, CurrentUserId, start, end);
            var response = AutoMapper.Mapper.Map<List<ExerciseHistoryResponse>>(oneRepMaxHistory);
            foreach (var volume in volumeHistory)
            {
                var match = response.FirstOrDefault(h => h.Time == volume.Time);
                if (match != null)
                {
                    match.TotalVolume = volume.TotalVolume;
                }
                else
                {
                    response.Add(new ExerciseHistoryResponse
                    {
                        ExerciseId = volume.ExerciseId,
                        Time = volume.Time,
                        TotalVolume = volume.TotalVolume
                    });
                }
            }
            return Ok(response.OrderBy(r => r.Time).ToArray());
        }

        [HttpPost("transfer")]
        public IActionResult TransferData([FromBody]TransferExerciseRequest request)
        {
            var fromExercise = trainingRepository.GetExercise(request.FromExerciseId, CurrentUserId, DateTimeOffset.MinValue);
            var toExercise = trainingRepository.GetExercise(request.ToExerciseId, CurrentUserId, DateTimeOffset.MinValue);
            if(fromExercise == null || toExercise == null)
            {
                return BadRequest();
            }
            if(fromExercise.UserId.HasValue && fromExercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            if (toExercise.UserId.HasValue && toExercise.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            trainingRepository.TransferExerciseData(CurrentUserId, request.FromExerciseId, request.ToExerciseId, request.TransferWorkouts, request.TransferRoutines, request.Transfer1RM);
            return Ok();
        }
    }
}