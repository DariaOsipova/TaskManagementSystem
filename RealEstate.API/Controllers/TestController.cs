using Microsoft.AspNetCore.Mvc;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                message = "API работает!",
                status = "OK",
                timestamp = DateTime.UtcNow 
            });
        }
    }
}