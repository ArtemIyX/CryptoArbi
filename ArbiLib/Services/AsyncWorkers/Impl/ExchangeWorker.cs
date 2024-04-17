using ArbiLib.Libs;
using ArbiLib.Services.Worker;
using ccxt;
using Newtonsoft.Json;

namespace ArbiLib.Services.AsyncWorkers.Impl
{
    public class ExchangeWorker(Exchange ExchangeObject, ArbiService InService) : AsyncWorker(InService)
    {
        private Exchange _exchange = ExchangeObject;
        public Tickers? Tickers;
        public TradingFees? Fees;

        public override void StartWork()
        {
            base.StartWork();

            _ = FetchFees();  
        }

        protected virtual async Task FetchFees()
        {
            await Task.Run(async () =>
            {
                Fees = await _exchange.FetchTradingFees();
            }, _cancellationTokenSource.Token);
        }

        protected override async Task DoWork()
        {
            Tickers = await _exchange.FetchTickers();
            _ = Parallel.ForEach(Tickers.Value.tickers, item =>
            {
                string tickerName = TickerLib.RemoveSemiColon(item.Key);
                if (TickerLib.IsUsdtPair(tickerName))
                {
                    string friendlySymbol = TickerLib.GetPureTicker(tickerName);
                    if (item.Value.ask != null && item.Value.bid != null)
                    {
                        Ticker ticker = item.Value;
                        Arbi.UpdateTicker(friendlySymbol,
                                                        _exchange,
                                                        ref ticker);
                    }
                }
            });
            await Task.Delay(1000);
        }
    }
}
