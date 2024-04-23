using ArbiDataLib.Data;
using ArbiReader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    [Route("api/tokens")]
    [ApiController]
    public class TokenController(ITokenService tokenService) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;

        [HttpGet("symbol")]
        public async Task<IActionResult> Get([FromQuery] string symbol)
        {
            try
            {
                IList<ArbiDataLib.Models.ExchangeTokenResponse> res = await _tokenService.GetBySymbol(symbol);
                if (res.Count == 0)
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
            catch (Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            try
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
            catch (Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }

        [HttpGet("arbi")]
        public async Task<IActionResult> ArbiFilterd([FromQuery] ArbiFilter filter)
        {
            try
            {
                filter.Amount = Math.Clamp(filter.Amount, 1, 200);
                List<ArbiItem> content = await _tokenService.GetArbi(filter);
                return this.OkData(content);
            }
            catch (Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }
    }
}
