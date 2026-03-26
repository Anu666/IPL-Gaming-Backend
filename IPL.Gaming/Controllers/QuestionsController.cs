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
    public class QuestionsController : BaseController
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        [Route("GetAllQuestions")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetAllQuestions()
        {
            try
            {
                var questions = await _questionService.GetAllQuestions();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetQuestionById/{questionId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetQuestionById(Guid questionId)
        {
            try
            {
                var question = await _questionService.GetQuestionById(questionId);
                if (question == null)
                {
                    return NotFound(new { message = $"Question with ID {questionId} not found" });
                }
                return Ok(question);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetQuestionsByMatchId/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Player)]
        public async Task<IActionResult> GetQuestionsByMatchId(Guid matchId)
        {
            try
            {
                var questions = await _questionService.GetQuestionsByMatchId(matchId);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateQuestion")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                if (string.IsNullOrWhiteSpace(request.QuestionText))
                    return BadRequest(new { message = "Question text is required" });

                if (request.MatchId == Guid.Empty)
                    return BadRequest(new { message = "A valid Match ID is required" });

                var question = QuestionMapper.ToQuestion(request);
                var createdQuestion = await _questionService.CreateQuestion(question);
                return CreatedAtAction(nameof(GetQuestionById), new { questionId = createdQuestion.Id }, createdQuestion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateQuestion")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateQuestion([FromBody] UpdateQuestionRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return BadRequest(new { message = "Request body with valid ID is required" });

                if (request.MatchId == Guid.Empty)
                    return BadRequest(new { message = "A valid Match ID is required" });

                var question = QuestionMapper.ToQuestion(request);
                var updatedQuestion = await _questionService.UpdateQuestion(question);
                return Ok(updatedQuestion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteQuestion/{questionId}/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> DeleteQuestion(Guid questionId, Guid matchId)
        {
            try
            {
                var result = await _questionService.DeleteQuestion(questionId, matchId);
                if (!result)
                {
                    return NotFound(new { message = $"Question with ID {questionId} not found or could not be deleted" });
                }
                return Ok(new { message = "Question deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
