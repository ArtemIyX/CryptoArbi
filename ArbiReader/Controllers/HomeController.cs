
using ArbiDataLib.Data;
using ArbiReader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await Task.Run(() =>
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
                    return this.InteralServerErrorData(ex);
                }
            });
        }
    }
}
