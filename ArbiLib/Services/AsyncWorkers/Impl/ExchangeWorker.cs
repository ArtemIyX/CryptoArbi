using ArbiLib.Libs;
using ArbiLib.Services.Worker;
using ccxt;

namespace ArbiLib.Services.AsyncWorkers.Impl
{
    public class ExchangeWorker(Exchange ExchangeObject, ArbiService Service) : AsyncWorker(Service)
    {
        private Exchange _exchange = ExchangeObject;
        private Tickers? _tickers;

        protected override Task DoWork()
        {
            return Task.Run(async () =>
            {
                _tickers = await _exchange.FetchTickers();
                _ = Parallel.ForEach(_tickers.Value.tickers, async item =>
                {
                    string ticker = TickerLib.RemoveSemiColon(item.Key);
                    if (TickerLib.IsUsdtPair(ticker))
                    {
                        ticker = TickerLib.GetPureTicker(ticker);
                        if (item.Value.ask != null && item.Value.bid != null)
                        {
                            ArbiServce.UpdateTicker(ticker,
                                                            _exchange,
                                                            item.Value.ask ?? 0.0,
                                                            item.Value.bid ?? 0.0,
                                                            ticker,
                                                            item.Value.quoteVolume ?? 0.0,
                                                            item.Value.askVolume ?? 0.0,
                                                            item.Value.bidVolume ?? 0.0);
                        }
                    }
                });
                await Task.Delay(1000);
            });
        }
    }
}
