using ArbiReader.Data;
using ArbiReader.Data.Responses;
using ArbiReader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    [Route("api/exchange")]
    [ApiController]
    public class ExchangeController(IExchangeService exchangeService) : ControllerBase
    {
        private readonly IExchangeService _exchangeService = exchangeService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IList<ArbiDataLib.Models.ExchangeEntityResponse> content = await _exchangeService.Get();
            return Ok(new BasicResponse()
            {
                Code = (int)HttpStatusCode.OK,
                Message = "",
                Data = new ExchangeListResponse(content),
                Success = true
            });
        }

        [HttpGet("working")]
        public async Task<IActionResult> GetWorking()
        {
            IList<ArbiDataLib.Models.ExchangeEntityResponse> content = await _exchangeService.Working();
            return Ok(new BasicResponse()
            {
                Code = (int)HttpStatusCode.OK,
                Message = "",
                Data = new ExchangeListResponse(content),
                Success = true
            });
        }
    }
}
