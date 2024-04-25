using ArbiBlazor.Extensions;
using ArbiDataLib.Data;
using ArbiDataLib.Models;
using System.Collections.Concurrent;

namespace ArbiBlazor.Services
{
    public interface IArbiService
    {
        public Task<IList<ArbiItemVisual>> GetArbiItems(ArbiFilter filter);
    }
    public class ArbiService(HttpClient http,
        IExchangeService exchangeService) : IArbiService
    {
        private readonly HttpClient _http = http;
        private readonly IExchangeService _exchangeService = exchangeService;
        private readonly string ArbiUrl = "/api/arbi";


        public async Task<IList<ArbiItemVisual>> GetArbiItems(ArbiFilter filter)
        {
            try
            {
                // Get arbi situations
                List<ArbiItem> list = [];
                IList<ExchangeEntityResponse> exchanges = [];
                IList<ExchangeUrlInfo> exchangeUrls = [];
                List<Task> globalTasks = [];

                // Get exchanges (with names)
                globalTasks.Add(Task.Run(async () =>
                {
                    exchanges = await _exchangeService.GetExchanges();
                }));
                // Get exchange urls 
                globalTasks.Add(Task.Run(async () =>
                {
                    exchangeUrls = await _exchangeService.GetExchangeUrlInfos();

                }));
                // Get arbi situatiins
                globalTasks.Add(Task.Run(async () =>
                {
                    BasicResponse? response = await _http.GetBasicAsync(ArbiUrl);
                    list = response.TryParseContent<List<ArbiItem>>() ?? [];

                }));
                await Task.WhenAll(globalTasks);
                ConcurrentBag<ArbiItemVisual> visal = [];


                // For each arbi
                foreach (var item in list)
                {

                    ArbiItem savedItem = item;
                    List<Task> localTasks = [];
                    string buyName = string.Empty;
                    string buyUrl = string.Empty;
                    string withdrawUrl = string.Empty;
                    string sellName = string.Empty;
                    string depositUrl = string.Empty;
                    string sellUrl = string.Empty;

                    // Get buy exchange info
                    localTasks.Add(Task.Run(() =>
                    {
                        ExchangeEntityResponse? buyExchange = exchanges.FirstOrDefault(x => x.Id == item.ExchangeId1);
                        ExchangeUrlInfo? buyInfo = exchangeUrls.FirstOrDefault(x => x.ExchangeId == item.ExchangeId1);
                        if (buyInfo is null) return;
                        buyName = buyExchange?.Name ?? "Unknown";
                        buyUrl = buyInfo.TradeURL;
                        withdrawUrl = buyInfo.WithdrawalURL;
                    }));
                    // Get sell exchange info
                    localTasks.Add(Task.Run(() =>
                    {
                        ExchangeEntityResponse? sellExchange = exchanges.FirstOrDefault(x => x.Id == item.ExchangeId2);
                        ExchangeUrlInfo? sellInfo = exchangeUrls.FirstOrDefault(x => x.ExchangeId == item.ExchangeId2);
                        if (sellInfo is null) return;
                        sellName = sellExchange?.Name ?? "Unknown";
                        sellUrl = sellInfo.TradeURL;
                        depositUrl = sellInfo.DepositURL;
                    }));
                    // Wait until both exchanges are finished
                    await Task.WhenAll(localTasks);
                    // Add to arbi item
                    visal.Add(new ArbiItemVisual(item,
                        buyName,
                        buyUrl,
                        withdrawUrl,
                        sellName,
                        depositUrl,
                        sellUrl));
                }
                return [.. visal.AsEnumerable().OrderByDescending(x => x.PriceDifferencePercentage)];
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
