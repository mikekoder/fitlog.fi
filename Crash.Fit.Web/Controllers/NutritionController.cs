using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Crash.Fit.Nutrition;
using Crash.Fit.Api.Models.Nutrition;
using Microsoft.Extensions.Logging;
using Crash.Fit.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class NutritionController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public NutritionController(INutritionRepository nutritionRepository, ILogRepository logger) : base(logger)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpGet("nutrients")]
        [AllowAnonymous]
        public IActionResult Nutrients()
        {
            var nutrients = nutritionRepository.GetNutrients().OrderBy(n => n.DefaultOrder ?? int.MaxValue).ThenBy(n => n.Name);
            var response = AutoMapper.Mapper.Map<NutrientResponse[]>(nutrients);
            return Ok(response);
        }
        [HttpGet("settings")]
        public IActionResult NutrientSettings()
        {
            var nutrients = nutritionRepository.GetNutrientSettings(CurrentUserId);
            var response = AutoMapper.Mapper.Map<NutrientSettingResponse[]>(nutrients);
            return Ok(response);
        }
        [HttpGet("goals/active")]
        public IActionResult ActiveGoal()
        {
            var goal = nutritionRepository.GetNutritionGoals(CurrentUserId).FirstOrDefault(g => g.Active);
            if(goal == null)
            {
                return Ok();
            }
            var response = AutoMapper.Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(response);
        }
        
        [HttpGet("goals")]
        public IActionResult ListGoals()
        {
            var goals = nutritionRepository.GetNutritionGoals(CurrentUserId);
            var response = AutoMapper.Mapper.Map<NutritionGoalResponse[]>(goals);

            return Ok(response);
        }
        [HttpGet("goals/{id}")]
        public IActionResult GetGoal(Guid id)
        {
            var goal = nutritionRepository.GetNutritionGoals(CurrentUserId).SingleOrDefault(g => g.Id == id);
            if(goal == null)
            {
                return NotFound();
            }
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            var response = AutoMapper.Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(response);
        }
        [HttpPost("goals")]
        public IActionResult CreateGoal([FromBody] NutritionGoalRequest request)
        {
            var goal = AutoMapper.Mapper.Map<NutritionGoalDetails>(request);
            goal.UserId = CurrentUserId;

            var goals = nutritionRepository.GetNutritionGoals(CurrentUserId);
            if (!goals.Any())
            {
                goal.Active = true;
            }
            nutritionRepository.CreateNutritionGoal(goal);

            var result = AutoMapper.Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(result);
        }
        [HttpPut("goals/{id}")]
        public IActionResult UpdateGoal(Guid id, [FromBody] NutritionGoalRequest request)
        {
            var goal = nutritionRepository.GetNutritionGoal(id);
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            AutoMapper.Mapper.Map(request, goal);
            nutritionRepository.UpdateNutritionGoal(goal);

            var result = AutoMapper.Mapper.Map<NutritionGoalResponse>(goal);

            return Ok(result);
        }
        [HttpPost("goals/{id}/activate")]
        public IActionResult ActivateGoal(Guid id)
        {
            var goal = nutritionRepository.GetNutritionGoal(id);
            if(goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            nutritionRepository.ActivateNutritionGoal(goal);
            return Ok();
        }
        [HttpDelete("goals/{id}")]
        public IActionResult DeleteGoal(Guid id)
        {
            var goal = nutritionRepository.GetNutritionGoal(id);
            if (goal.UserId != CurrentUserId)
            {
                return Unauthorized();
            }

            nutritionRepository.DeleteNutritionGoal(goal);
            return Ok();
        }
        [HttpPut("settings")]
        public IActionResult UpdateSettings([FromBody] NutrientSettingRequest[] request)
        {
            var settings = request.Select((s, index) => 
            {
                var setting = AutoMapper.Mapper.Map<NutrientSetting>(s);
                setting.UserId = CurrentUserId;
                setting.Order = index;
                return setting;
            });

            nutritionRepository.SaveNutrientSettings(settings);

            var response = AutoMapper.Mapper.Map<NutrientSettingResponse[]>(settings);
            return Ok(response);
        }

        /*
        [HttpGet]
        [Route("daily-intakes")]
        public IEnumerable<DailyIntake> DailyIntakes(Gender gender, DateTime dateOfBirth)
        {
            var age = DateTime.Now - dateOfBirth;
            return nutritionRepository.GetDailyIntakes(gender, age);
        }
        */
    }
}