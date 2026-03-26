using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Mappers;
using IPL.Gaming.Common.Models.Requests;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnswersController : BaseController
    {
        private readonly IUserAnswerService _userAnswerService;

        public UserAnswersController(IUserAnswerService userAnswerService)
        {
            _userAnswerService = userAnswerService;
        }

        [HttpGet]
        [Route("GetAllUserAnswers")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetAllUserAnswers()
        {
            try
            {
                var userAnswers = await _userAnswerService.GetAllUserAnswers();
                return Ok(userAnswers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUserAnswerById/{userAnswerId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetUserAnswerById(Guid userAnswerId)
        {
            try
            {
                var userAnswer = await _userAnswerService.GetUserAnswerById(userAnswerId);
                if (userAnswer == null)
                {
                    return NotFound(new { message = $"UserAnswer with ID {userAnswerId} not found" });
                }
                return Ok(userAnswer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUserAnswersByMatchId/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetUserAnswersByMatchId(Guid matchId)
        {
            try
            {
                var userAnswers = await _userAnswerService.GetUserAnswersByMatchId(matchId);
                return Ok(userAnswers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUserAnswerByMatchAndUser/{matchId}/{userId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetUserAnswerByMatchAndUser(Guid matchId, Guid userId)
        {
            try
            {
                var userAnswer = await _userAnswerService.GetUserAnswerByMatchAndUser(matchId, userId);
                if (userAnswer == null)
                {
                    return NotFound(new { message = $"No answers found for user {userId} in match {matchId}" });
                }
                return Ok(userAnswer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateUserAnswer")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CreateUserAnswer([FromBody] CreateUserAnswerRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                if (request.MatchId == Guid.Empty)
                    return BadRequest(new { message = "A valid Match ID is required" });

                if (request.UserId == Guid.Empty)
                    return BadRequest(new { message = "A valid User ID is required" });

                if (request.Answers == null || request.Answers.Count == 0)
                    return BadRequest(new { message = "At least one answer is required" });

                var userAnswer = UserAnswerMapper.ToUserAnswer(request);
                var created = await _userAnswerService.CreateUserAnswer(userAnswer);
                return CreatedAtAction(nameof(GetUserAnswerById), new { userAnswerId = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateUserAnswer")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateUserAnswer([FromBody] UpdateUserAnswerRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return BadRequest(new { message = "Request body with valid ID is required" });

                if (request.MatchId == Guid.Empty)
                    return BadRequest(new { message = "A valid Match ID is required" });

                var userAnswer = UserAnswerMapper.ToUserAnswer(request);
                var updated = await _userAnswerService.UpdateUserAnswer(userAnswer);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteUserAnswer/{userAnswerId}/{matchId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> DeleteUserAnswer(Guid userAnswerId, Guid matchId)
        {
            try
            {
                var result = await _userAnswerService.DeleteUserAnswer(userAnswerId, matchId);
                if (!result)
                {
                    return NotFound(new { message = $"UserAnswer with ID {userAnswerId} not found or could not be deleted" });
                }
                return Ok(new { message = "UserAnswer deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
