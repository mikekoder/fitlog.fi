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

namespace Crash.Fit.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SettingsController : ApiControllerBase
    {
        private readonly INutritionRepository nutritionRepository;
        public SettingsController(INutritionRepository nutritionRepository, ILogRepository logger):base(logger)
        {
            this.nutritionRepository = nutritionRepository;
        }
        [HttpPut("home")]
        public IActionResult UpdateHomeSettings([FromBody]HomeSettingsRequest request)
        {
            nutritionRepository.SaveHomeNutrients(CurrentUserId, request.Nutrients.Where(n => n.HasValue).Select(n => n.Value).ToArray());
            return Ok();
        
        }
    }
}
