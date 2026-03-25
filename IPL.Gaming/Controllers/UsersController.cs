using IPL.Gaming.Attributes;
using IPL.Gaming.Common.Models.CosmosDB;
using IPL.Gaming.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    //[ApiKey]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet]
        [Route("{userId}")]
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
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest(new { message = "User data is required" });
                }

                if (string.IsNullOrEmpty(user.Name))
                {
                    return BadRequest(new { message = "User name is required" });
                }

                var createdUser = await _userService.CreateUser(user);
                return CreatedAtAction(nameof(GetUserById), new { userId = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                if (user == null || user.Id == Guid.Empty)
                {
                    return BadRequest(new { message = "User data with valid ID is required" });
                }

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
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
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
    }
}
