using Microsoft.AspNetCore.Mvc;

namespace ArbiReader.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> SimpleGet()
        {
            return Ok(new { });
        }
    }
}
