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
    public class MusclesController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public MusclesController(ITrainingRepository trainingRepository)
        {
            this.trainingRepository = trainingRepository;
        }

        [HttpGet]
        [Route("groups")]
        public IEnumerable<MuscleGroup> ListGroups()
        {
            var muscleGroups = trainingRepository.GetMuscleGroups();
            return muscleGroups;
        }
    }
}