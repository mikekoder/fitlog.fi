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
    public class MusclesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public MusclesController(ITrainingRepository trainingRepository, ILogRepository logger) : base(logger)
        {
            this.trainingRepository = trainingRepository;
        }

        [HttpGet("groups")]
        public IActionResult ListGroups()
        {
            var muscleGroups = trainingRepository.GetMuscleGroups().OrderBy(g => g.Name);

            var response = AutoMapper.Mapper.Map<MuscleGroupResponse[]>(muscleGroups);
            return Ok(response);
        }
    }
}