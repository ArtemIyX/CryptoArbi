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

        [HttpGet("sym/{symbol}")]
        public async Task<IActionResult> GetBySymbol(string symbol)
        {
            try
            {
                IList<ArbiDataLib.Models.ExchangeTokenResponse> res = await _tokenService.GetTokens(symbol);
                if (res.Count <= 0)
                {
                    return this.NotFoundData($"Token with symbol '{symbol}' not found");
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
                ArbiDataLib.Models.ExchangeTokenResponse? token = await _tokenService.GetToken(id);
                if (token is null)
                {
                    return this.NotFoundData($"Token with id '{id}' not found");
                }
                return this.OkData(token);
            }
            catch (Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }


    }
}
