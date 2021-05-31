using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fitlog.Training;
using Fitlog.Api.Models.Training;
using Fitlog.Logging;
using AutoMapper;

namespace Fitlog.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class MusclesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public MusclesController(ITrainingRepository trainingRepository, ILogRepository logger, IMapper mapper) : base(logger, mapper)
        {
            this.trainingRepository = trainingRepository;
        }

        [HttpGet("groups")]
        public IActionResult ListGroups()
        {
            var muscleGroups = trainingRepository.GetMuscleGroups().OrderBy(g => g.Name);

            var response = Mapper.Map<MuscleGroupResponse[]>(muscleGroups);
            return Ok(response);
        }
    }
}