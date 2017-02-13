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
        [Route("search")]
        public IEnumerable<RoutineMinimal> List()
        {
            var routines = trainingRepository.SearchRoutines(CurrentUserId);
            return routines;
        }
        [HttpGet]
        [Route("{id}")]
        public RoutineDetails Details(Guid id)
        {
            var routine = trainingRepository.GetRoutine(id);
            return routine;
        }
        [HttpPost]
        [Route("")]
        public RoutineDetails Create(RoutineRequest request)
        {
            var routine = AutoMapper.Mapper.Map<RoutineDetails>(request);
            trainingRepository.CreateRoutine(routine);
            return routine;
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(Guid id, RoutineRequest request)
        {
            var routine = trainingRepository.GetRoutine(id);
            if (routine.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, routine);
            trainingRepository.UpdateRoutine(routine);
            return Ok(routine);
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
    }
}