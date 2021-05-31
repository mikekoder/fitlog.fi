using AutoMapper;
using Fitlog.Api.Models;
using Fitlog.Api.Models.Home;
using Fitlog.Logging;
using Fitlog.Nutrition;
using Fitlog.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;

namespace Fitlog.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SettingsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        private readonly ISettingsRepository settingsRepository;
        public SettingsController(INutritionRepository nutritionRepository, ISettingsRepository settingsRepository, ILogRepository logger, IMapper mapper) : base(logger, mapper)
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
