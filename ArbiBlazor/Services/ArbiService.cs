using ArbiBlazor.Extensions;
using ArbiDataLib.Data;
using ArbiDataLib.Models;
using System.Collections.Concurrent;
using System.Text;

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
                BasicResponse? response = await _http.GetBasicAsync(ArbiUrl);
                List<ArbiItem>? list = response
                    .TryParseContent<List<ArbiItem>>();
                if (list is null) return [];

                ConcurrentBag<ArbiItemVisual> visal = [];
                List<Task> globalTasks = [];

                // For each arbi
                foreach (var item in list)
                {
                    // Async ItemVisual task
                    globalTasks.Add(Task.Run(async () =>
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
                        localTasks.Add(Task.Run(async () =>
                        {
                            ExchangeEntityResponse? buyExchange = await _exchangeService.GetExchange(exchangeId: item.ExchangeId1);
                            ExchangeUrlInfo? buyInfo = await _exchangeService.GetExchangeUrlInfo(item.ExchangeId1);
                            if (buyInfo is null) return;
                            buyName = buyExchange?.Name ?? "Unknown";
                            buyUrl = buyInfo.TradeURL;
                            withdrawUrl = buyInfo.WithdrawalURL;
                        }));
                        // Get sell exchange info
                        localTasks.Add(Task.Run(async () =>
                        {
                            ExchangeEntityResponse? sellExchange = await _exchangeService.GetExchange(exchangeId: item.ExchangeId2);
                            ExchangeUrlInfo? sellInfo = await _exchangeService.GetExchangeUrlInfo(item.ExchangeId2);
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
                    }));
                }
                await Task.WhenAll(globalTasks);
                return visal.AsEnumerable().OrderByDescending(x => x.PriceDifferencePercentage).ToList();
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
