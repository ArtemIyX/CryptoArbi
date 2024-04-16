using ArbiLib.Services;
using ccxt;
using Newtonsoft.Json;
using System.Globalization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        CultureInfo culture = new CultureInfo("en-US");
        culture.NumberFormat.NumberDecimalSeparator = ".";

        // Set the current thread's culture to use the new culture
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;

        ArbiLib.Services.ArbiService service = new ArbiService(new List<Exchange>());
        List<ExchangeWorker> workers =
        [
            new ExchangeWorker(new Binance(), service),
            new ExchangeWorker(new Mexc(), service),
            new ExchangeWorker(new Kucoin(), service),
            //new ExchangeWorker(new Gateio(), service),
            new ExchangeWorker(new Kraken(), service),
            new ExchangeWorker(new Bybit(), service),
            new ExchangeWorker(new Huobi(), service),
            new ExchangeWorker(new Bitstamp(), service),
        ];

        foreach (ExchangeWorker worker in workers)
        {
            _ = worker.StartWork();
        }
        _ = Display(service);
        await WaitForKeyPress();
        Console.WriteLine("Key pressed. Program exiting...");
        foreach (ExchangeWorker worker in workers)
        {
            worker.Dispose();
        }
        workers.Clear();
    }

    static async Task Display(ArbiLib.Services.ArbiService service)
    {
        while(true)
        {
            Console.Clear();
            var res = service.FindOpportunities(45.0, 50);
            List<string> list = res.Select(x => x.ToString()).ToList();
            await Console.Out.WriteLineAsync(Newtonsoft.Json.JsonConvert.SerializeObject(list, Formatting.Indented));
            await Task.Delay(1000);
        }
    }
    static async Task WaitForKeyPress()
    {
        // Ожидаем нажатие клавиши асинхронно
        await Task.Run(() => Console.ReadKey(true));
    }
}
