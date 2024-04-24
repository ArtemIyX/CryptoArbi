using ArbiDataLib.Data;
using ArbiReader.Services;
using Microsoft.AspNetCore.Mvc;

namespace ArbiReader.Controllers
{
    [Route("api/arbi")]
    [ApiController]
    public class ArbiController(ITokenService tokenService) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ArbiFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(filter.ForbiddenBuy))
                {
                    if (!filter.IsValidForbiddenBuy())
                    {
                        return this.BadData($"'Buy' must match regex: {ArbiFilter.ForbiddenRegex()}");
                    }
                }
                if (!string.IsNullOrEmpty(filter.ForbiddenSell))
                {
                    if (!filter.IsValidForbiddenSell())
                    {
                        return this.BadData($"'Sell' must match regex: {ArbiFilter.ForbiddenRegex()}");
                    }
                }
                filter.Amount = Math.Clamp(filter.Amount, 1, 200);
                IList<ArbiItem> content = await _tokenService.GetArbi(filter);
                if (content.Count <= 0)
                {
                    return this.NotFoundData($"No arbi situations with selected filter");
                }
                return this.OkData(content);
            }
            catch (Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }
        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetArbi(string symbol)
        {
            try
            {
                if(string.IsNullOrEmpty(symbol))
                {
                    return this.BadData($"Symbol must be not null or empty");
                }
                IList<ArbiItem> content = await _tokenService.GetArbiBySymbol(symbol);
                if(content.Count <= 0)
                {
                    return this.NotFoundData($"No arbi situations with symbol '{symbol}'");
                }
                return this.OkData(content);
            }
            catch (Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }
    }
}
