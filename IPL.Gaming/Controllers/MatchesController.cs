using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : BaseController
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        [Route("GetAllMatches")]
        public async Task<IActionResult> GetAllMatches()
        {
            try
            {
                var matches = await _matchService.GetAllMatches();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetMatchById/{matchId}")]
        public async Task<IActionResult> GetMatchById(Guid matchId)
        {
            try
            {
                var match = await _matchService.GetMatchById(matchId);
                if (match == null)
                {
                    return NotFound(new { message = $"Match with ID {matchId} not found" });
                }
                return Ok(match);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [HttpGet("GetMatchesByTeamName/{teamName}")]
        public async Task<IActionResult> GetMatchesByTeamName(string teamName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teamName))
                {
                    return BadRequest(new { message = "Team name is required" });
                }

                var matches = await _matchService.GetMatchesByTeamName(teamName);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving matches", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("CreateMatch")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> CreateMatch([FromBody] Match match)
        {
            try
            {
                if (match == null)
                {
                    return BadRequest(new { message = "Match data is required" });
                }

                if (string.IsNullOrEmpty(match.MatchName))
                {
                    return BadRequest(new { message = "Match name is required" });
                }

                var createdMatch = await _matchService.CreateMatch(match);
                return CreatedAtAction(nameof(GetMatchById), new { matchId = createdMatch.Id }, createdMatch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateMatch")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateMatch([FromBody] Match match)
        {
            try
            {
                if (match == null || match.Id == Guid.Empty)
                {
                    return BadRequest(new { message = "Match data with valid ID is required" });
                }

                var updatedMatch = await _matchService.UpdateMatch(match);
                return Ok(updatedMatch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("DeleteMatch/{matchId}")]
        [RequireRole(UserRole.Admin, UserRole.SuperAdmin)]
        public async Task<IActionResult> DeleteMatch(Guid matchId)
        {
            try
            {
                var result = await _matchService.DeleteMatch(matchId);
                if (!result)
                {
                    return NotFound(new { message = $"Match with ID {matchId} not found or could not be deleted" });
                }
                return Ok(new { message = "Match deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
