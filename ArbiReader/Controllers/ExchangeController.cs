
using ArbiDataLib.Data;
using ArbiReader.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArbiReader.Controllers
{
    [Route("api/exchanges")]
    [ApiController]
    public class ExchangeController(IServiceScopeFactory serviceScopeFactory) : ControllerBase
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IExchangeService exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
            return this.OkData(await exchangeService.Get());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IExchangeService exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
            ArbiDataLib.Models.ExchangeEntityResponse? ex = await exchangeService.Get(id);
            if (ex is null)
            {
                return NotFound(new BasicResponse()
                {
                    Data = null,
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = $"Exchange with id '{id}' not found"
                });
            }
            return this.OkData(ex);
        }

        [HttpGet("working")]
        public async Task<IActionResult> GetWorking()
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            IExchangeService exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
            return this.OkData(await exchangeService.Working());
        }
           
    }
}
