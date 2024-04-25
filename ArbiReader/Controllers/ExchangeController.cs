
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

        [HttpGet("u")]
        public async Task<IActionResult> GetInfo()
        {
            return await Task.Run(() =>
            {
                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    IExchangeInfoService exInfoService = scope.ServiceProvider.GetRequiredService<IExchangeInfoService>();
                    var res = exInfoService.GetExchangeInfos();
                    if (res.Length <= 0)
                    {
                        return this.NotFoundData($"No exchange info");
                    }
                    return this.OkData(res);
                }
                catch (Exception ex)
                {
                    return this.InteralServerErrorData(ex);
                }
            });
        }

        [HttpGet("u/{id}")]
        public async Task<IActionResult> GetInfo(string id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using IServiceScope scope = _serviceScopeFactory.CreateScope();
                    IExchangeInfoService exInfoService = scope.ServiceProvider.GetRequiredService<IExchangeInfoService>();
                    var res = exInfoService.GetExchangeInfo(id);
                    if (res is null)
                    {
                        return this.NotFoundData($"Exchange information with id '{id}' not found");
                    }
                    return this.OkData(res);
                }
                catch (Exception ex)
                {
                    return this.InteralServerErrorData(ex);
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IExchangeService exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
                IExchangeInfoService exInfoService = scope.ServiceProvider.GetRequiredService<IExchangeInfoService>();
                List<ArbiDataLib.Models.ExchangeEntityResponse> exchanges = (await exchangeService.Get())
                    .Select(item =>
                    {
                        item.Url = exInfoService.GetExchangeInfo(item.Id)?.HomeURL;
                        return item;
                    }).ToList();
                if (exchanges.Count <= 0)
                {
                    return this.NotFoundData($"No exchanges");
                }
                return this.OkData(exchanges);
            }
            catch(Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IExchangeService exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
                ArbiDataLib.Models.ExchangeEntityResponse? ex = await exchangeService.Get(id);

                if (ex is null)
                {
                    return this.NotFoundData($"Exchange with id '{id}' not found");
                }
                IExchangeInfoService exInfoService = scope.ServiceProvider.GetRequiredService<IExchangeInfoService>();
                ex.Url = exInfoService.GetExchangeInfo(ex.Id)?.HomeURL;
                return this.OkData(ex);
            }
            catch(Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }

        [HttpGet("working")]
        public async Task<IActionResult> GetWorking()
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IExchangeService exchangeService = scope.ServiceProvider.GetRequiredService<IExchangeService>();
                IExchangeInfoService exInfoService = scope.ServiceProvider.GetRequiredService<IExchangeInfoService>();
                List<ArbiDataLib.Models.ExchangeEntityResponse> working = (await exchangeService.Working())
                    .Select(item =>
                    {
                        item.Url = exInfoService.GetExchangeInfo(item.Id)?.HomeURL;
                        return item;
                    }).ToList();
                if (working.Count <= 0)
                {
                    return this.NotFoundData($"No working exchanges");
                }
                return this.OkData(working);
            }
            catch(Exception ex)
            {
                return this.InteralServerErrorData(ex);
            }
        }
           
    }
}
