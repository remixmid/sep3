using Microsoft.AspNetCore.Mvc;

namespace AspCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "AspCoreWebAPI",
                Version = "1.0.0"
            });
        }
    }
}
