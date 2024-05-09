using ArbiBlazor.Extensions;
using ArbiDataLib.Data;
using ArbiDataLib.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace ArbiBlazor.Services
{

    public interface IArbiService
    {
        public Task<IList<ArbiItemVisual>> GetArbiItems(ArbiFilter filter);
        public Task<IList<ExchangeTokenVisual>> GetArbiSituation(string symbol);
    }
    public class ArbiService(HttpClient http,
        IExchangeService exchangeService,
        IFilterContainer filterContainer) : IArbiService
    {
        private readonly HttpClient _http = http;
        private readonly IExchangeService _exchangeService = exchangeService;
        private readonly IFilterContainer _filterContainer = filterContainer;
        private readonly string ArbiUrl = "/api/arbi";
        private readonly string TokenUrl = "api/tokens/sym/";

        private async Task<ExchangeInfoData> GetExchangeDataAsync()
        {
            IList<Task> tasks = [];
            ExchangeInfoData res = new ExchangeInfoData();
            IList<ExchangeEntityResponse> exchanges = [];
            IList<ExchangeUrlInfo> exchangeUrls = [];
            // Get exchanges (with names)
            tasks.Add(Task.Run(async () =>
            {
                res.Exchanges = await _exchangeService.GetExchanges();
            }));
            // Get exchange urls 
            tasks.Add(Task.Run(async () =>
            {
                res.ExchangeUrls = await _exchangeService.GetExchangeUrlInfos();
            }));
            await Task.WhenAll(tasks);
            return res;
        }

        public async Task<IList<ExchangeTokenVisual>> GetArbiSituation(string symbol)
        {
            try
            {
                IList<Task> globalTasks = [];
                ExchangeInfoData exchangeData = new();
                IList<ExchangeTokenResponse> tokens = [];
                globalTasks.Add(Task.Run(async () =>
                {
                    exchangeData = await GetExchangeDataAsync();
                }));
                // Get arbi situatiins
                globalTasks.Add(Task.Run(async () =>
                {
                    BasicResponse? response = await _http.GetBasicAsync($"{TokenUrl}{symbol}");
                    tokens = response.TryParseContent<List<ExchangeTokenResponse>>() ?? [];
                }));
                await Task.WhenAll(globalTasks);

                IList<ExchangeTokenVisual> visual = [];
                foreach (ExchangeTokenResponse token in tokens)
                {
                    ExchangeTokenResponse savedItem = token;

                    ExchangeEntityResponse? exchange = exchangeData.Exchanges.FirstOrDefault(x => x.Id == savedItem.ExchangeId);
                    if (exchange is null) continue;
                    ExchangeUrlInfo? exchangeInfo = exchangeData.ExchangeUrls.FirstOrDefault(x => x.ExchangeId == savedItem.ExchangeId);
                    if(exchangeInfo is null) continue;

                    visual.Add(new ExchangeTokenVisual(token,
                        exchange.Name,
                        exchangeInfo.TradeURL,
                        exchangeInfo.DepositURL,
                        exchangeInfo.WithdrawalURL));
                }

                return visual;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<IList<ArbiItemVisual>> GetArbiItems(ArbiFilter filter)
        {
            try
            {
                // Get arbi situations
                IList<ArbiItem> arbiList = [];
                IList<Task> globalTasks = [];
                ExchangeInfoData exchangeData = new ExchangeInfoData();
                globalTasks.Add(Task.Run(async () =>
                {
                    exchangeData = await GetExchangeDataAsync();
                }));
                // Get arbi situatiins
                globalTasks.Add(Task.Run(async () =>
                {
                    _filterContainer.CurrentFilter.ForbiddenBuy = _filterContainer.MakeForbiddenString(_filterContainer.BuyExchanges);
                    _filterContainer.CurrentFilter.ForbiddenSell = _filterContainer.MakeForbiddenString(_filterContainer.SellExchanges);
                    string url = QueryHelpers.AddQueryString(ArbiUrl, _filterContainer.CurrentFilter.ToArgsDictionary());
                    BasicResponse? response = await _http.GetBasicAsync(url);
                    arbiList = response.TryParseContent<List<ArbiItem>>() ?? [];
                }));
                await Task.WhenAll(globalTasks);
                IList<ArbiItemVisual> visal = [];


                // For each arbi
                foreach (ArbiItem item in arbiList)
                {
                    ArbiItem savedItem = item;
                    IList<Task> localTasks = [];
                    string buyName = string.Empty;
                    string buyUrl = string.Empty;
                    string withdrawUrl = string.Empty;
                    string sellName = string.Empty;
                    string depositUrl = string.Empty;
                    string sellUrl = string.Empty;

                    // Get buy exchange info
                    localTasks.Add(Task.Run(() =>
                    {
                        ExchangeEntityResponse? buyExchange = exchangeData.Exchanges.FirstOrDefault(x => x.Id == savedItem.ExchangeId1);
                        ExchangeUrlInfo? buyInfo = exchangeData.ExchangeUrls.FirstOrDefault(x => x.ExchangeId == savedItem.ExchangeId1);
                        if (buyInfo is null) return;
                        buyName = buyExchange?.Name ?? "Unknown";
                        buyUrl = buyInfo.TradeURL;
                        withdrawUrl = buyInfo.WithdrawalURL;
                    }));
                    // Get sell exchange info
                    localTasks.Add(Task.Run(() =>
                    {
                        ExchangeEntityResponse? sellExchange = exchangeData.Exchanges.FirstOrDefault(x => x.Id == savedItem.ExchangeId2);
                        ExchangeUrlInfo? sellInfo = exchangeData.ExchangeUrls.FirstOrDefault(x => x.ExchangeId == savedItem.ExchangeId2);
                        if (sellInfo is null) return;
                        sellName = sellExchange?.Name ?? "Unknown";
                        sellUrl = sellInfo.TradeURL;
                        depositUrl = sellInfo.DepositURL;
                    }));
                    // Wait until both exchanges are finished
                    await Task.WhenAll(localTasks);
                    IList<ArbiSameNetwork> sameNetworks = ArbiItem.HasSameNetworks(item.AskNetworks, item.BidNetworks);
                    if(sameNetworks.Count > 0)
                    {
                        ArbiSameNetwork? bestNet = sameNetworks.OrderBy(x => x.Ask.Fee).FirstOrDefault();
                        if(bestNet is not null)
                        {
                            // Add to arbi item
                            visal.Add(new ArbiItemVisual(item,
                                buyName,
                                buyUrl,
                                withdrawUrl,
                                sellName,
                                depositUrl,
                                sellUrl,
                                bestNet));
                        }
                    }
                    
                }
                return visal.OrderByDescending(x => x.PriceDifferencePercentage).ToList();
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
