using ArbiLib.Libs;
using ArbiLib.Models;
using ArbiLib.Services.Worker;
using ccxt;
using Newtonsoft.Json;

namespace ArbiLib.Services.AsyncWorkers.Impl
{
    public class ExchangeWorker(Exchange ExchangeObject) : AsyncWorker()
    {
        protected Exchange _exchange = ExchangeObject;
        public Tickers? Tickers;

        public override void StartWork()
        {
            base.StartWork();
        }

        protected override async Task DoWork()
        {
            Tickers = await _exchange.FetchTickers();
            _ = Parallel.ForEach(Tickers.Value.tickers, item =>
            {
                string tickerName = TickerLib.RemoveSemiColon(item.Value.symbol ?? "");

                if (TickerLib.IsUsdtPair(tickerName))
                {
                    string friendlySymbol = TickerLib.GetPureTicker(tickerName);
                    if (item.Value.ask != null && item.Value.bid != null)
                    {
                        UpdateTicker(friendlySymbol, item.Value);
                    }
                }
            });
            await Task.Delay(2000);
        }

        protected virtual void UpdateTicker(string friendlySymbol, in Ticker ticker)
        {
            /*Arbi.UpdateTicker(friendlySymbol,
                                                        _exchange,
                                                        ref ticker);*/
        }
    }
}
