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
                    await _exchange.FetchTickers();
                    await Task.Delay(10000, cancellationToken);
                    /*Parallel.ForEach(_tickers.Value.tickers, item =>
                    {
                        string ticker = TickerLib.RemoveSemiColon(item.Key);
                        if (TickerLib.IsUsdtPair(ticker))
                        {
                            ticker = TickerLib.GetPureTicker(ticker);
                            if (item.Value.ask != null && item.Value.bid != null)
                            {
                                _arbiServce.UpdateTicker(ticker,
                                                                _exchange,
                                                                (double)item.Value.ask,
                                                                (double)item.Value.bid,
                                                                ticker);
                            }
                        }
                    });*/
                    // Запускаем Garbage Collector для освобождения ненужной памяти
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                catch (Exception ex)
                {

                }
               
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            try
            {
                _task?.Wait();
            }catch(Exception) { }
                    

            _cancellationTokenSource.Dispose();
        }

    }
}
