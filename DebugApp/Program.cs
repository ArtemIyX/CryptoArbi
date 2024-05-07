using ArbiLib.Services;
using ccxt;
using DebugApp;
using Newtonsoft.Json;
using System.Globalization;

internal class Program
{
    private static readonly Dictionary<string, string[]> NetworkSynonyms = new()
        {
            { "ERC20", new string[] { "ERC-20", "ETH", "ERC 20" } },
            { "BEP20", new string[] { "BEP-20, BEP", "BSC", "ERC 20"} },
            { "SOL", new string[] {"SOLANA"} }
        };
    private static async Task Main(string[] args)
    {
        Exchange ex = new Bingx();
        ex.apiKey = "NjzKvnwsHxfy3VoMw5qygtCYiGhU7Q4BkvZLGmdWzxKv2ydcwAVRCmzQSkw5YIicGqq0oJ1ETocBkNH8ntd6Sg";
        ex.secret = "3OUqZGZnx6xedw9zQUC78MqmOa1ZcE4v3S7hZ026SibWB2JFErcHjMJc7u6Gow2rp74qreAyMuGBiGfxjw";
        await Console.Out.WriteLineAsync("Loading...");
        await Console.Out.WriteLineAsync(ex.id);
        //Tickers tickersContainer = await ex.FetchTickers();
        //await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(tickersContainer, formatting: Formatting.Indented));
        Currencies currenciesContainer = await ex.FetchCurrencies();
        HashSet<string> networks = new HashSet<string>();
        foreach (var item in currenciesContainer.currencies)
        {
            if(item.Value.networks is not null)
            {
                foreach (var network in item.Value.networks)
                {
                    await Console.Out.WriteLineAsync($"[{item.Key}]{network.Key} {(network.Value.fee ?? 0.0).ToString()}");
                    //await Console.Out.WriteLineAsync($"[{item.Key}]{network.Key} {}");
                }
            }
        }
        //await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(currenciesContainer, formatting: Formatting.Indented));
        await Console.Out.WriteLineAsync("Finished");
    }

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
