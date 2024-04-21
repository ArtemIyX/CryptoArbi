using ArbiReader.Data;
using ArbiReader.Data.Responses;
using ArbiReader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController(ITokenService tokenService) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;

        [HttpGet("symbol")]
        public async Task<IActionResult> Get([FromQuery] string symbol)
        {
            IList<ArbiDataLib.Models.ExchangeTokenResponse> res = await _tokenService.GetBySymbol(symbol);
            if(res.Count == 0)
            {
                return NotFound(new BasicResponse()
                {
                    Data = null,
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = $"Token with symbol '{symbol}' not found"
                });
            }
            return this.OkData(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            ArbiDataLib.Models.ExchangeTokenResponse? token = await _tokenService.GetById(id);
            if (token is null)
            {
                return NotFound(new BasicResponse()
                {
                    Data = null,
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = $"Token with id '{id}' not found"
                });
            }
            return this.OkData(token);
        }

        [HttpGet("arbi")]
        public async Task<IActionResult> ArbiFilterd([FromQuery] ArbiFilter filter)
        {
            filter.Amount = Math.Clamp(filter.Amount, 1, 200);
            List<ArbiItem> content = await _tokenService.GetArbi(filter);
            return this.OkData(content);
        }
    }
}
