using ArbiLib.Libs;
using ccxt;

namespace ArbiLib.Services
{
    public class ExchangeWorker(ccxt.Exchange ExchangeObject, ArbiService ArbiServce) : IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Task? _task = null;
        private ccxt.Exchange _exchange = ExchangeObject;
        private readonly ArbiService _arbiServce = ArbiServce;
        private Tickers? _tickers;

        public void StartWork()
        {
            _task = Task.Run(async () => await DoWork(_cancellationTokenSource.Token));

        }

        private async Task DoWork(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _tickers = await _exchange.FetchTickers();
                    foreach (var item in _tickers.Value.tickers)
                    {
                        string ticker = TickerLib.RemoveSemiColon(item.Key);
                        if (TickerLib.IsUsdtPair(ticker))
                        {
                            ticker = TickerLib.GetPureTicker(ticker);
                            if (item.Value.ask != null && item.Value.bid != null)
                            {
                                _arbiServce.AddArbi(ticker, new Models.Arbi(
                                    _exchange,
                                    (double)item.Value.ask,
                                    (double)item.Value.bid,
                                    ticker));
                            }
                        }

                    }

                }
                catch (Exception ex)
                {

                }
                await Task.Delay(100, cancellationToken);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _task.Wait();
            _cancellationTokenSource.Dispose();
        }

    }
}
