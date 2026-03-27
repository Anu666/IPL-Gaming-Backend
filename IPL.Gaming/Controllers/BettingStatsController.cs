using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class BettingStatsController : BaseController
    {
        private readonly IBettingStatsService _bettingStatsService;

        public BettingStatsController(IBettingStatsService bettingStatsService)
        {
            _bettingStatsService = bettingStatsService;
        }

        /// <summary>
        /// Manually triggers betting stats calculation for all questions in a match.
        /// Safe to call multiple times — results are overwritten each run.
        /// </summary>
        [HttpPost]
        [Route("Calculate/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CalculateForMatch(Guid matchId)
        {
            try
            {
                await _bettingStatsService.CalculateAndUpdateBettingStats(matchId);
                return Ok(new { message = $"Betting stats calculated successfully for match {matchId}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
