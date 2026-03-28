using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Enums;
using IPL.Gaming.Common.Mappers;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Common.Models.Requests;
using IPL.Gaming.Services;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiKey]
    [Route("api/users")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserCacheService _userCacheService;
        private readonly ITransactionService _transactionService;

        public UsersController(IUserService userService, UserCacheService userCacheService, ITransactionService transactionService)
        {
            _userService = userService;
            _userCacheService = userCacheService;
            _transactionService = transactionService;
        }

        [HttpGet]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet]
        [Route("{userId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {userId} not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUserByApiKey/{apiKey}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> GetUserByApiKey(string apiKey)
        {
            try
            {
                var user = await _userService.GetUserByApiKey(apiKey);
                if (user == null)
                {
                    return NotFound(new { message = "User not found or inactive" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request body is required" });

                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { message = "User name is required" });

                var user = UserMapper.ToUser(request);
                var createdUser = await _userService.CreateUser(user);
                return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return BadRequest(new { message = "Request body with valid ID is required" });

                var existingUser = await _userService.GetUserById(request.Id);
                if (existingUser == null)
                    return NotFound(new { message = $"User with ID {request.Id} not found" });

                var user = UserMapper.ApplyUpdate(request, existingUser);
                var updatedUser = await _userService.UpdateUser(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("{userId}")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                var userToDelete = await _userService.GetUserById(userId);
                if (userToDelete == null)
                {
                    return NotFound(new { message = $"User with ID {userId} not found" });
                }

                if (userToDelete.Role == UserRole.SuperAdmin)
                {
                    return BadRequest(new { message = "Super Admin users cannot be deleted" });
                }

                var result = await _userService.DeleteUser(userId);
                if (!result)
                {
                    return NotFound(new { message = $"User with ID {userId} not found or could not be deleted" });
                }
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Manually refresh the user API key cache
        /// </summary>
        [HttpPost("RefreshCache")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> RefreshCache()
        {
            try
            {
                await _userCacheService.RefreshCache();
                var count = _userCacheService.GetCachedUserCount();
                return Ok(new { message = $"Cache refreshed successfully. {count} users cached." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error refreshing cache", error = ex.Message });
            }
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        [HttpGet("CacheStats")]
        [RequireRole(UserRole.SuperAdmin)]
        public IActionResult GetCacheStats()
        {
            try
            {
                var count = _userCacheService.GetCachedUserCount();
                return Ok(new { cachedUsers = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting cache stats", error = ex.Message });
            }
        }

        /// <summary>
        /// Get the currently authenticated user's own details (any authenticated user)
        /// </summary>
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            var user = CurrentUser;
            if (user == null)
            {
                return Unauthorized(new { message = "Authenticated user not found in context." });
            }
            return Ok(user);
        }

        /// <summary>
        /// Update credits for a specific user without modifying any other fields
        /// </summary>
        [HttpPatch("{userId}/credits")]
        [RequireRole(UserRole.SuperAdmin)]
        public async Task<IActionResult> UpdateCredits(Guid userId, [FromBody] UpdateCreditsRequest request)
        {
            try
            {
                var existingUser = await _userService.GetUserById(userId);
                if (existingUser == null)
                {
                    return NotFound(new { message = $"User with ID {userId} not found" });
                }

                if (request.Operation == CreditsOperation.Deposit)
                {
                    var creditChange = (double)request.Credits;
                    existingUser.Credits += request.Credits;
                    await _transactionService.CreateTransaction(new Transaction
                    {
                        UserId               = userId,
                        OverallCreditChange  = creditChange,
                        Status               = TransactionStatus.Completed,
                        Type                 = TransactionType.Deposit
                    });
                }
                else if (request.Operation == CreditsOperation.Withdrawal)
                {
                    var creditChange = -(double)request.Credits;
                    existingUser.Credits -= request.Credits;
                    await _transactionService.CreateTransaction(new Transaction
                    {
                        UserId               = userId,
                        OverallCreditChange  = creditChange,
                        Status               = TransactionStatus.Completed,
                        Type                 = TransactionType.Withdrawal
                    });
                }
                else // Override
                {
                    var zeroOut = -(double)existingUser.Credits;
                    var newValue = (double)request.Credits;

                    await _transactionService.CreateTransaction(new Transaction
                    {
                        UserId               = userId,
                        OverallCreditChange  = zeroOut,
                        Status               = TransactionStatus.Completed,
                        Type                 = TransactionType.AdminOverride
                    });
                    await _transactionService.CreateTransaction(new Transaction
                    {
                        UserId               = userId,
                        OverallCreditChange  = newValue,
                        Status               = TransactionStatus.Completed,
                        Type                 = TransactionType.AdminOverride
                    });

                    existingUser.Credits = request.Credits;
                }

                var updatedUser = await _userService.UpdateUser(existingUser);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
