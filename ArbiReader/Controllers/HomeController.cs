
using ArbiDataLib.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(new BasicResponse()
                {
                    Data = null,
                    Code = (int)HttpStatusCode.OK,
                    Message = "Pong",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return this.InteralServerErrorData(new Exception());
            }
        }
    }
}
