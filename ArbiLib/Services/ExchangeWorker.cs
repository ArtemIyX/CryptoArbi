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
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
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
                                    _arbiServce.UpdateTicker(ticker,
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
                    }
                    catch (Exception ex)
                    {

                    }

                }
            });

        }

        public void Dispose()
        {
            Console.WriteLine(_exchange.name.ToString() + " - Dispose");
            _cancellationTokenSource.Cancel();
            try
            {
                _task?.Wait();
            }
            catch (Exception) { }


            _cancellationTokenSource.Dispose();
        }

    }
}
