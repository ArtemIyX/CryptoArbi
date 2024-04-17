using ArbiLib.Services;
using ccxt;
using Nethereum.Util;
using Newtonsoft.Json;
using System.Globalization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        CultureInfo culture = new CultureInfo("en-US");
        culture.NumberFormat.NumberDecimalSeparator = ".";
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;

        ArbiLib.Services.ArbiService service = new ArbiService(new List<Exchange>()
        {
            new Binance(),
            new Mexc(),
            new Kucoin(),
            new Kraken(),
            //new Bybit(),
            new Huobi(),
            new Bitstamp(),
            new Coinbase(),
            new Okx(),
            new Bitget(),
            new Bitfinex(),
        });
        service.MaxOportunities = 100;
        service.MinProfitPercent = 0.1;
        service.MaxProfitPercent = 65.0;
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

    static async Task Display(ArbiLib.Services.ArbiService service)
    {
        while(true)
        {
            Console.Clear();
            var res = service.Oportunities.OrderByDescending(x => x.PercentDiff()).ToArray();
            foreach(var item in res)
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
