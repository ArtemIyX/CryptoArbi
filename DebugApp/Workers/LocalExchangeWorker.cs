using ArbiLib.Services.AsyncWorkers.Impl;
using ccxt;

namespace DebugApp.Workers
{
    public class LocalExchangeWorker(Exchange ExchangeObject, LocalArbiService ArbiService) : ExchangeWorker(ExchangeObject)
    {
        public LocalArbiService LocalArbiService { get; private set; } = ArbiService;

        protected override void UpdateTicker(string friendlySymbol, in Ticker ticker)
        {
            LocalArbiService.UpdateTicker(friendlySymbol,
                                                        _exchange,
                                                        ticker);
        }
    }
}
