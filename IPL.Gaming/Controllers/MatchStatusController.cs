using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Mappers;
using IPL.Gaming.Common.Models.Requests;
using IPL.Gaming.Common.Models.Responses;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class MatchStatusController : BaseController
    {
        private readonly IMatchStatusService _matchStatusService;
        private readonly IMatchService _matchService;

        public MatchStatusController(IMatchStatusService matchStatusService, IMatchService matchService)
        {
            _matchStatusService = matchStatusService;
            _matchService = matchService;
        }

        [HttpGet]
        [Route("GetAllMatchStatuses")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Player)]
        public async Task<IActionResult> GetAllMatchStatuses()
        {
            try
            {
                var statuses = await _matchStatusService.GetAllMatchStatuses();
                var summary = statuses.Select(s => new MatchStatusSummary
                {
                    Id = s.Id,
                    MatchId = s.MatchId,
                    Status = s.Status,
                    MatchCommenceStartDate = s.MatchCommenceStartDate,
                });
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetMatchStatusById/{matchStatusId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> GetMatchStatusById(Guid matchStatusId)
        {
            try
            {
                var status = await _matchStatusService.GetMatchStatusById(matchStatusId);
                if (status == null)
                    return NotFound(new { message = $"MatchStatus with ID {matchStatusId} not found" });

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetMatchStatusByMatchId/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin, UserRole.Player)]
        public async Task<IActionResult> GetMatchStatusByMatchId(Guid matchId)
        {
            try
            {
                var status = await _matchStatusService.GetMatchStatusByMatchId(matchId);
                if (status == null)
                    return NotFound(new { message = $"No status found for match {matchId}" });

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateMatchStatus")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CreateMatchStatus([FromBody] CreateMatchStatusRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                if (request.MatchId == Guid.Empty)
                    return BadRequest(new { message = "A valid Match ID is required" });

                var matchStatus = MatchStatusMapper.ToMatchStatusRecord(request);
                var created = await _matchStatusService.CreateMatchStatus(matchStatus);
                return CreatedAtAction(nameof(GetMatchStatusById), new { matchStatusId = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateMatchStatus")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateMatchStatus([FromBody] UpdateMatchStatusRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return BadRequest(new { message = "Request body with valid ID is required" });

                if (request.MatchId == Guid.Empty)
                    return BadRequest(new { message = "A valid Match ID is required" });

                var matchStatus = MatchStatusMapper.ToMatchStatusRecord(request);
                var updated = await _matchStatusService.UpdateMatchStatus(matchStatus);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteMatchStatus/{matchStatusId}/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> DeleteMatchStatus(Guid matchStatusId, Guid matchId)
        {
            try
            {
                var result = await _matchStatusService.DeleteMatchStatus(matchStatusId, matchId);
                if (!result)
                    return NotFound(new { message = $"MatchStatus with ID {matchStatusId} not found or could not be deleted" });

                return Ok(new { message = "MatchStatus deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("MarkMatchComplete/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> MarkMatchComplete(Guid matchId)
        {
            try
            {
                var updated = await _matchStatusService.MarkMatchComplete(matchId);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("MarkTransactionsSettled/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> MarkTransactionsSettled(Guid matchId)
        {
            try
            {
                var updated = await _matchStatusService.MarkTransactionsSettled(matchId);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("MarkDone/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> MarkDone(Guid matchId)
        {
            try
            {
                var updated = await _matchStatusService.MarkDone(matchId);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("MarkArchived/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> MarkArchived(Guid matchId)
        {
            try
            {
                var updated = await _matchStatusService.MarkArchived(matchId);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("RecalculateLeaderboard/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> RecalculateLeaderboard(Guid matchId)
        {
            try
            {
                var updated = await _matchStatusService.RecalculateLeaderboard(matchId);
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("OverrideMatchStatus/{matchId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> OverrideMatchStatus(Guid matchId, [FromBody] OverrideStatusRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                var updated = await _matchStatusService.OverrideMatchStatus(matchId, request.Status);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch]
        [Route("UpdateMatchStartTime/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateMatchStartTime(Guid matchId, [FromBody] UpdateMatchStartTimeRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                // Fetch both records
                var matchStatus = await _matchStatusService.GetMatchStatusByMatchId(matchId);
                if (matchStatus == null)
                    return NotFound(new { message = $"No status record found for match {matchId}" });

                var match = await _matchService.GetMatchById(matchId);
                if (match == null)
                    return NotFound(new { message = $"Match with ID {matchId} not found" });

                // Update MatchStatus record
                matchStatus.MatchCommenceStartDate = request.MatchCommenceStartDate;
                var updatedStatus = await _matchStatusService.UpdateMatchStatus(matchStatus);

                // Keep Match record in sync so the backend lock check is consistent
                match.MatchCommenceStartDate = request.MatchCommenceStartDate;
                await _matchService.UpdateMatch(match);

                return Ok(updatedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }

    public class OverrideStatusRequest
    {
        public MatchStatus Status { get; set; }
    }

    public class UpdateMatchStartTimeRequest
    {
        public DateTime MatchCommenceStartDate { get; set; }
    }
}
