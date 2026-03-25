using Microsoft.AspNetCore.Mvc;

namespace IPL.Gaming.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Health()
        {
            return Ok("ok");
        }
    }
}   

