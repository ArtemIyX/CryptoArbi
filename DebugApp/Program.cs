using ccxt;
using DebugApp;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

internal class Program
{
    public class OrderDataItem
    {
        public double Price { get; set; }
        public double Vol { get; set; }
    }
    public class OrderData
    {
        [JsonProperty("ask")]
        public List<List<double>> ask { get; set; }

        [JsonProperty("bid")]
        public List<List<double>> bid { get; set; }

        public static IList<OrderDataItem> Convert(List<List<double>> list)
        {
            return list.Select(x => new OrderDataItem()
            {
                Price = x[0],
                Vol = x[1]
            }).ToList();
        }
    }
    private static readonly Dictionary<string, string[]> NetworkSynonyms = new()
        {
            { "ERC20", new string[] { "ERC-20", "ETH", "ERC 20" } },
            { "BEP20", new string[] { "BEP-20, BEP", "BSC", "ERC 20"} },
            { "SOL", new string[] {"SOLANA"} }
        };

    private static async Task Main(string[] args)
    {
        Exchange exchange = new ccxt.Bitfinex();
        await Console.Out.WriteLineAsync(exchange.id);
        //var fetched = await exchange.FetchTickers();
        var fetched = await exchange.FetchOrderBook("BTC/USDT");
        //var list = fetched.tickers.Select(x => new {sym= x.Value.symbol, ask= x.Value.ask, bid=x.Value.bid}).ToList();
        await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(fetched, Formatting.Indented));

        return;
        await Console.Out.WriteLineAsync("Loading");
        var sym = "SWASH/USDT";
        var a = FetchOrderBook(new Kucoin(), sym);
        var b = FetchOrderBook(new Mexc(), sym);
        await Task.WhenAll(a, b);
        OrderData res = new OrderData()
        {
            ask = a.Result.asks,
            bid = b.Result.bids
        };
        await File.WriteAllTextAsync("C:\\Users\\SystemX\\Downloads\\data.json", JsonConvert.SerializeObject(res));
        await Console.Out.WriteLineAsync("finished");
        string text = await File.ReadAllTextAsync("C:\\Users\\SystemX\\Downloads\\data.json");
        OrderData orderData = JsonConvert.DeserializeObject<OrderData>(text);
        IList<OrderDataItem> ask = OrderData.Convert(orderData.ask);
        IList<OrderDataItem> bid = OrderData.Convert(orderData.bid);
        double profitBefore = Percent(ask.OrderBy(x => x.Price).First().Price, bid.OrderByDescending(x => x.Price).First().Price);
        double halfProfit = profitBefore / 2;
        await Console.Out.WriteLineAsync($"Profit before: {profitBefore}");
        await Console.Out.WriteLineAsync($"Half profit: {halfProfit}");

        int askNum = 1;
        int bidNum = 1;
        double askVolume = SumVol(ask, askNum);
        double bidVolume = SumVol(bid, bidNum);
        double askAvg = AveragePrice(ask, askNum);
        double bidAvg = AveragePrice(bid, bidNum);
        double currentProfit = Percent(askAvg, bidAvg);
        await Console.Out.WriteLineAsync($"Ask avg: {askAvg}, bid avg: {bidAvg}");

        while (true)
        {
            if (askNum >= ask.Count)
            {
                await Console.Out.WriteLineAsync($"Ask num ({askNum}) => ask.Count");
                break;
            }
            if (bidNum >= bid.Count)
            {
                await Console.Out.WriteLineAsync($"Bid num ({bidNum}) => ask.Count");
                break;
            }
            if (currentProfit <= halfProfit)
            {
                await Console.Out.WriteLineAsync($"Current profit {currentProfit} <= {halfProfit} half profit");
                break;
            }
            if (askVolume > bidVolume)
            {
                await Console.Out.WriteLineAsync($"Ask volume {askVolume} > bid volume {bidVolume}");
                bidNum++;
                bidVolume = SumVol(bid, bidNum);
                bidAvg = AveragePrice(bid, bidNum);
                currentProfit = Percent(askAvg, bidAvg);
                continue;
            }
            if(bidVolume > askVolume)
            {
                await Console.Out.WriteLineAsync($"Bid volume {bidVolume} > ask volume {askVolume}");
                askNum++;
                askVolume = SumVol(ask, askNum);
                askAvg = AveragePrice(ask, askNum);
                currentProfit = Percent(askAvg, bidAvg);
                continue;
            }
        }
        double resultVolume = Math.Min(askVolume, bidVolume);
        double buyPrice = resultVolume * askAvg;
        double sellPrice = resultVolume * bidAvg;
        await Console.Out.WriteLineAsync($"Volume {resultVolume:F3}");
        await Console.Out.WriteLineAsync($"Budget {buyPrice:F2}$");
        await Console.Out.WriteLineAsync($"Buy [{ask[0].Price}] - [{ask[askNum-1].Price}] (avg {askAvg:F5}) -> -{buyPrice:F2}$");
        await Console.Out.WriteLineAsync($"Sell [{bid[0].Price}] - [{bid[askNum - 1].Price}] (avg {bidAvg:F5}) -> +{sellPrice:F2}$");
        await Console.Out.WriteLineAsync($"Profit: {sellPrice-buyPrice:F2}$");
    }

    public static double AveragePrice(IList<OrderDataItem> list, int num)
        => list.Take(num).Average(x => x.Price);

    public static double SumVol(IList<OrderDataItem> list, int num)
        => list.Take(num).Sum(x => x.Vol);

    static double Percent(double oldValue, double newValue)
        => ((newValue - oldValue) / oldValue) * 100;


    public static async Task<OrderBook> FetchOrderBook(Exchange ex, string sym)
        => await ex.FetchOrderBook(sym);
    

    public static bool IsNetworkEqual(string first, string second)
    {
        first = first.ToUpper();
        second = second.ToUpper();
        // Перебираем все синонимы первого слова
        foreach (var synonym in NetworkSynonyms.GetValueOrDefault(first, new string[0]))
        {
            // Если второе слово совпадает с каким-либо синонимом первого слова, возвращаем true
            if (synonym.Equals(second, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        // Перебираем все синонимы второго слова
        foreach (var synonym in NetworkSynonyms.GetValueOrDefault(second, new string[0]))
        {
            // Если первое слово совпадает с каким-либо синонимом второго слова, возвращаем true
            if (synonym.Equals(first, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return first.Equals(second, StringComparison.OrdinalIgnoreCase);
    }

    static async Task Work()
    {
        CultureInfo culture = new CultureInfo("en-US");
        culture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        LocalArbiService service = new LocalArbiService(new List<Exchange>()
        {
            new Binance(),
            new Mexc(),
            new Kucoin(),
            new Kraken(),
            //new Bybit(),
            new Huobi(),
            new Gateio(),
            new Bitstamp(),
            new Coinbase(),
            new Okx(),
            new Bitmex(),
            /*new Bitget(),
            new Bitfinex(),*/
        });
        service.MinProfitPercent = 0.0;
        service.MaxProfitPercent = 100;
        service.MinBidVolumeUsdt = 0;
        service.MinAskVolumeUsdt = 0;
        service.MinDayVolumeUsdt = 0;
        service.StartWorkers();
        _ = Display(service);

        await WaitForKeyPress();
        Console.WriteLine("Key pressed. Program exiting...");
        service.Dispose();
        GC.Collect();
        await Console.Out.WriteLineAsync("Press line");
        Console.ReadLine();
    }
    static async Task Display(LocalArbiService service)
    {
        while (true)
        {

            Console.Clear();
            await Console.Out.WriteLineAsync(service.OportunityList.Count.ToString());
            var res = service.OportunityList.OrderByDescending(x => x.PercentDiff()).Take(15).ToArray();
            foreach (var item in res)
            {
                await Console.Out.WriteLineAsync(item.ToString());
                //await Console.Out.WriteLineAsync();
            }
            //await Console.Out.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(res, Formatting.Indented));
            await Task.Delay(2500);
        }
    }
    static async Task WaitForKeyPress()
    {
        // Ожидаем нажатие клавиши асинхронно
        await Task.Run(() => Console.ReadKey(true));
    }
}
