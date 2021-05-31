using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fitlog.Logging;
using Microsoft.AspNetCore.Authorization;
using Fitlog.Training;
using Fitlog.Api.Models.Training;
using AutoMapper;

namespace Fitlog.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TrainingController : ApiControllerBase
    {
        private readonly ITrainingRepository trainingRepository;
        public TrainingController(ITrainingRepository trainingRepository, ILogRepository logger, IMapper mapper) : base(logger, mapper)
        {
            this.trainingRepository = trainingRepository;
        }
        
        [HttpGet("goals/active")]
        public IActionResult ActiveGoal()
        {
            var goal = trainingRepository.GetTrainingGoals(CurrentUserId).FirstOrDefault(g => g.Active);
            if(goal == null)
            {
                return Ok();
            }
            var response = Mapper.Map<TrainingGoalResponse>(goal);

            return Ok(response);
        }
        
        [HttpGet("goals")]
        public IActionResult ListGoals()
        {
            var goals = trainingRepository.GetTrainingGoals(CurrentUserId);
            var response = Mapper.Map<TrainingGoalResponse[]>(goals);

            return Ok(response);
        }
        [HttpGet("goals/{id}")]
        public IActionResult GetGoal(Guid id)
        {
            var goal = trainingRepository.GetTrainingGoals(CurrentUserId).SingleOrDefault(g => g.Id == id);
            if(goal == null)
            {
                return NotFound();
            }
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            var response = Mapper.Map<TrainingGoalResponse>(goal);

            return Ok(response);
        }
        [HttpPost("goals")]
        public IActionResult CreateGoal([FromBody] TrainingGoalRequest request)
        {
            var goal = Mapper.Map<TrainingGoalDetails>(request);
            goal.UserId = CurrentUserId;

            trainingRepository.CreateTrainingGoal(goal);

            var result = Mapper.Map<TrainingGoalResponse>(goal);

            return Ok(result);
        }
        [HttpPut("goals/{id}")]
        public IActionResult UpdateGoal(Guid id, [FromBody] TrainingGoalRequest request)
        {
            var goal = trainingRepository.GetTrainingGoal(id);
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            Mapper.Map(request, goal);
            trainingRepository.UpdateTrainingGoal(goal);

            var result = Mapper.Map<TrainingGoalResponse>(goal);

            return Ok(result);
        }
        [HttpPost("goals/{id}/activate")]
        public IActionResult ActivateGoal(Guid id)
        {
            var goal = trainingRepository.GetTrainingGoal(id);
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            trainingRepository.ActivateTrainingGoal(goal);
            return Ok();
        }
        [HttpDelete("goals/{id}")]
        public IActionResult DeleteGoal(Guid id)
        {
            var goal = trainingRepository.GetTrainingGoal(id);
            if (goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            trainingRepository.DeleteTrainingGoal(goal);
            return Ok();
        }
    }
}