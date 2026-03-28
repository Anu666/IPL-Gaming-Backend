using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : BaseController
    {
        private readonly IMatchStatusService _matchStatusService;

        public LeaderboardController(IMatchStatusService matchStatusService)
        {
            _matchStatusService = matchStatusService;
        }

        [HttpGet]
        [Route("GetLeaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            try
            {
                var allStatuses = await _matchStatusService.GetAllMatchStatuses();

                var latest = allStatuses
                    .Where(r => r.Status == MatchStatus.Done
                             && r.Leaderboard != null
                             && r.Leaderboard.Count > 0
                             && r.CompletedAt.HasValue)
                    .OrderByDescending(r => r.CompletedAt)
                    .FirstOrDefault();

                if (latest == null)
                    return Ok(Array.Empty<LeaderboardEntry>());

                return Ok(latest.Leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
