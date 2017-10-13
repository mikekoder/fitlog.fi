using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Crash.Fit.Api.Models.Users;
using Crash.Fit.Web.Models.Auth;
using Microsoft.AspNetCore.Antiforgery;
using Crash.Fit.Logging;
using Crash.Fit.Feedback;
using Crash.Fit.Api.Models.Feedback;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Crash.Fit.Web.Controllers
{
    [Route("api/[controller]")]
    public class FeedbackController : ApiControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackController(UserManager<User> userManager,SignInManager<User> signInManager, IFeedbackRepository feedbackRepository, ILogRepository logger) : base(logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _feedbackRepository = feedbackRepository;
        }

        [HttpGet("bugs")]
        public IActionResult GetBugs()
        {
            var bugs = _feedbackRepository.GetFeedback(FeedbackType.Bug);
            var result = AutoMapper.Mapper.Map<FeedbackSummaryResponse[]>(bugs);

            return Ok(result);
        }
        [HttpGet("improvements")]
        public IActionResult GetImprovements()
        {
            var improvements = _feedbackRepository.GetFeedback(FeedbackType.Improvement);
            var result = AutoMapper.Mapper.Map<FeedbackSummaryResponse[]>(improvements);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody]FeedbackRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var feedback = AutoMapper.Mapper.Map<Feedback.FeedbackDetails>(model);
            feedback.UserId = CurrentUserId;
            feedback.Created = DateTimeOffset.Now;
            _feedbackRepository.CreateFeedback(feedback);

            var result = AutoMapper.Mapper.Map<FeedbackDetailsResponse>(feedback);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]FeedbackRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var feedback = _feedbackRepository.GetFeedback(id);
            if(feedback == null)
            {
                return NotFound();
            }
            if(feedback.UserId != CurrentUserId)
            {
                return Unauthorized();
            }
            //feedback.Type = model.Type;
            feedback.Title = model.Title;
            feedback.Description = model.Description;
            _feedbackRepository.UpdateFeedback(feedback);

            var result = AutoMapper.Mapper.Map<FeedbackDetailsResponse>(feedback);
            return Ok(result);
        }
        [HttpGet("votes")]
        public async Task<IActionResult> GetVotes()
        {
            var votes = _feedbackRepository.GetVotes(CurrentUserId);
            return Ok(votes);
        }

        [HttpPost("{id}/vote")]
        public async Task<IActionResult> Vote(Guid id)
        {
            if(_feedbackRepository.UserHasVoted(id, CurrentUserId))
            {
                return BadRequest();
            }
            _feedbackRepository.VoteFeedback(id, CurrentUserId);
            return Ok();
        }
    }
}
