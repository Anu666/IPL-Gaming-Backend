using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Mappers;
using IPL.Gaming.Common.Models.CosmosDB;
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
        private readonly IMatchStatusService _matchStatusService;

        public QuestionsController(IQuestionService questionService, IMatchStatusService matchStatusService)
        {
            _questionService = questionService;
            _matchStatusService = matchStatusService;
        }

        private (bool isValid, string errorMessage) ValidateQuestionOptions(List<Option> options, float credits)
        {
            // Check if options list exists
            if (options == null || options.Count == 0)
                return (false, "At least 2 options are required");

            // Check minimum options
            if (options.Count < 2)
                return (false, "At least 2 options are required for a valid question");

            // Check maximum options
            if (options.Count > 10)
                return (false, "Maximum 10 options allowed per question");

            // Check for duplicate option IDs
            var optionIds = options.Select(o => o.Id).ToList();
            if (optionIds.Count != optionIds.Distinct().Count())
                return (false, "Duplicate option IDs found. Each option must have a unique ID");

            // Check all option IDs are positive
            if (options.Any(o => o.Id <= 0))
                return (false, "All option IDs must be positive integers");

            // Check all option texts are non-empty
            if (options.Any(o => string.IsNullOrWhiteSpace(o.OptionText)))
                return (false, "All options must have non-empty text");

            // Check for duplicate option texts (case-insensitive)
            var optionTexts = options.Select(o => o.OptionText.Trim().ToLower()).ToList();
            if (optionTexts.Count != optionTexts.Distinct().Count())
                return (false, "Duplicate option texts found. Each option must have unique text");

            // Check credits
            if (credits <= 0)
                return (false, "Credits must be greater than 0");

            return (true, string.Empty);
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

                // Validate options and credits
                var (isValid, errorMessage) = ValidateQuestionOptions(request.Options, request.Credits);
                if (!isValid)
                    return BadRequest(new { message = errorMessage });

                var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(request.MatchId);
                if (matchStatus != null && matchStatus.Status != MatchStatus.NotStarted)
                    return StatusCode(403, new { message = "Questions are locked. Match status must be Not Started to add questions." });

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

                // Validate options and credits
                var (isValid, errorMessage) = ValidateQuestionOptions(request.Options, request.Credits);
                if (!isValid)
                    return BadRequest(new { message = errorMessage });

                var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(request.MatchId);
                if (matchStatus != null && matchStatus.Status != MatchStatus.NotStarted)
                    return StatusCode(403, new { message = "Questions are locked. Match status must be Not Started to update questions." });

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
                var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(matchId);
                if (matchStatus != null && matchStatus.Status != MatchStatus.NotStarted)
                    return StatusCode(403, new { message = "Questions are locked. Match status must be Not Started to delete questions." });

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

        [HttpPatch]
        [Route("SetCorrectAnswer/{questionId}/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> SetCorrectAnswer(Guid questionId, Guid matchId, [FromBody] SetCorrectAnswerRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(matchId);
                if (matchStatus == null || matchStatus.Status != MatchStatus.MatchCompleted)
                    return StatusCode(403, new { message = "Correct answer can only be set when the match status is Match Completed." });

                var question = await _questionService.GetQuestionById(questionId);
                if (question == null)
                    return NotFound(new { message = $"Question with ID {questionId} not found" });

                // Validate that the correct option ID exists in the question's options
                if (request.CorrectOptionId.HasValue)
                {
                    var optionExists = question.Options?.Any(o => o.Id == request.CorrectOptionId.Value) ?? false;
                    if (!optionExists)
                        return BadRequest(new { message = $"Option ID {request.CorrectOptionId} does not exist in this question's options" });
                }

                question.CorrectOptionId = request.CorrectOptionId;
                var updated = await _questionService.UpdateQuestion(question);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class SetCorrectAnswerRequest
    {
        public int? CorrectOptionId { get; set; }
    }
}
