using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Crash.Fit.Nutrition;
using Crash.Fit.Api.Models.Home;
using Crash.Fit.Logging;
using Crash.Fit.Api.Models;
using Crash.Fit.Settings;
using System.Reflection;

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SettingsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        private readonly ISettingsRepository settingsRepository;
        public SettingsController(INutritionRepository nutritionRepository, ISettingsRepository settingsRepository, ILogRepository logger):base(logger)
        {
            this.nutritionRepository = nutritionRepository;
            this.settingsRepository = settingsRepository;
        }
        [HttpPut("home")]
        public IActionResult UpdateHomeSettings([FromBody]HomeSettingsRequest request)
        {
            nutritionRepository.SaveHomeNutrients(CurrentUserId, request.Nutrients.Where(n => n.HasValue).Select(n => n.Value).ToArray());
            return Ok();
        
        }

        [HttpGet("{key}")]
        public IActionResult GetSettings(string key)
        {
            var settings = settingsRepository.GetSettings(CurrentUserId, key);
            if(settings == null)
            {
                settings = SettingsUtils.CreateDefault(key);
            }
            return Ok(settings);
        }

        [HttpPut]
        public IActionResult UpdateSettings([FromBody]SettingsRequest request)
        {
            var type = typeof(SettingsKeyAttribute).Assembly.GetTypes().FirstOrDefault(t => t.GetCustomAttribute<SettingsKeyAttribute>()?.Key == request.Key);
            if(type == null)
            {
                return BadRequest("Unknown key");
            }

            var settings = settingsRepository.GetSettings(CurrentUserId, request.Key);
            if(settings == null)
            {
                settings = SettingsUtils.CreateDefault(request.Key);
            }

            SettingsUtils.Merge(settings, request.Data);

            settingsRepository.UpdateSettings(CurrentUserId, request.Key, settings);

            return Ok();
        }
    }
}
