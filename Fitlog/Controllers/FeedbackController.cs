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
using Fitlog.Api.Models.Users;
using Fitlog.Web.Models.Auth;
using Microsoft.AspNetCore.Antiforgery;
using Fitlog.Logging;
using Fitlog.Feedback;
using Fitlog.Api.Models.Feedback;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;

namespace Fitlog.Web.Controllers
{
    [Route("api/[controller]")]
    public class FeedbackController : ApiControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackController(IFeedbackRepository feedbackRepository, ILogRepository logger, IMapper mapper) : base(logger, mapper)
        {
            _feedbackRepository = feedbackRepository;
        }

        [HttpGet("bugs")]
        public IActionResult GetBugs()
        {
            var bugs = _feedbackRepository.GetFeedback(FeedbackType.Bug);
            var result = Mapper.Map<FeedbackSummaryResponse[]>(bugs);

            return Ok(result);
        }
        [HttpGet("improvements")]
        public IActionResult GetImprovements()
        {
            var improvements = _feedbackRepository.GetFeedback(FeedbackType.Improvement);
            var result = Mapper.Map<FeedbackSummaryResponse[]>(improvements);

            return Ok(result);
        }

        [HttpPost("")]
        public IActionResult Create([FromBody]FeedbackRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var feedback = Mapper.Map<Feedback.FeedbackDetails>(model);
            feedback.UserId = CurrentUserId;
            feedback.Created = DateTimeOffset.Now;
            _feedbackRepository.CreateFeedback(feedback);

            var result = Mapper.Map<FeedbackDetailsResponse>(feedback);
            return Ok(result);
        }
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]FeedbackRequest model)
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

            var result = Mapper.Map<FeedbackDetailsResponse>(feedback);
            return Ok(result);
        }
        [HttpGet("votes")]
        public IActionResult GetVotes()
        {
            var votes = _feedbackRepository.GetVotes(CurrentUserId);
            return Ok(votes);
        }

        [HttpPost("{id}/vote")]
        public IActionResult Vote(Guid id)
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
