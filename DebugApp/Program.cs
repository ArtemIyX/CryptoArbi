using ArbiLib.Services;
using ccxt;
using DebugApp;
using Newtonsoft.Json;
using System.Globalization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Exchange ex = new Bitget();
        ex.apiKey = "";
        ex.secret = "";
        await Console.Out.WriteLineAsync("Loading...");
        var res = await ex.FetchCurrencies();
        foreach(var item in res.currencies)
        {
            if(item.Value.active == false || item.Value.active is null)
            {
                await Console.Out.WriteLineAsync(item.Value.id);
            }
        }
        //await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(res, Formatting.Indented));
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
