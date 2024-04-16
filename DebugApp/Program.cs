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
            //new Mexc(),
            new Kucoin(),
            new Kraken(),
            new Bybit(),
            new Huobi(),
            new Bitstamp()
        });
        service.MaxOportunities = 10;

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
            }
            //await Console.Out.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(res, Formatting.Indented));
            await Task.Delay(250);
        }
    }
    static async Task WaitForKeyPress()
    {
        // Ожидаем нажатие клавиши асинхронно
        await Task.Run(() => Console.ReadKey(true));
    }
}
