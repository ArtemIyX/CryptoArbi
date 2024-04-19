using ArbiLib.Models;
using ArbiLib.Services.AsyncWorkers.Impl;
using ccxt;
using DebugApp.Workers;
using System.Collections.Concurrent;
using System.Diagnostics;
namespace DebugApp
{
    public class LocalArbiService : IDisposable
    {
        public ConcurrentDictionary<string, ConcurrentBag<Arbi>> ArbiDictionary { get; private set; } = [];
        public List<Exchange> Exchanges { get; private set; }
        public List<LocalExchangeWorker> ExchangeWorkers { get; private set; } = [];

        public ArbiCollector Collector { get; private set; }
        public ArbiProcessor Processor { get; private set; }

        public ConcurrentQueue<ArbiOportunity> ArbiOportunitiesQueue { get; private set; } = [];
        public List<ArbiOportunity> OportunityList { get; set; } = [];

        public double MinProfitPercent { get; set; } = 1.0;
        public double MaxProfitPercent { get; set; } = 50.0;
        public double MinAskVolumeUsdt { get; set; } = 500.0;
        public double MinBidVolumeUsdt { get; set; } = 500.0;
        public double MinDayVolumeUsdt { get; set; } = 0;

        public LocalArbiService(List<Exchange> InExchanges)
        {
            Exchanges = InExchanges;
            Collector = new ArbiCollector(this);
            Processor = new ArbiProcessor(this);
        }

        public void Dispose()
        {
            foreach (var ex in ExchangeWorkers)
            {
                ex.Dispose();
            }
            Collector.Dispose();
            Processor.Dispose();

            ArbiDictionary.Clear();
            Exchanges.Clear();
            ExchangeWorkers.Clear();
        }

        public void StartWorkers()
        {
            foreach (var ex in Exchanges)
            {
                LocalExchangeWorker worker = new LocalExchangeWorker(ex, this);
                ExchangeWorkers.Add(worker);
                worker.StartWork();
            }
            Collector.StartWork();
            Processor.StartWork();
        }

        public void UpdateTicker(string Key, ccxt.Exchange ExchangeObject, in Ticker Ticker)
        {
            ConcurrentBag<Arbi> arbiList = ArbiDictionary.GetOrAdd(Key, _ => new ConcurrentBag<Arbi>());
            Arbi? exchangeArbi = arbiList.FirstOrDefault(x => x.ExchangeObject == ExchangeObject);
            if (exchangeArbi is not null)
            {
                exchangeArbi.Ask = Ticker.ask ?? 0.0;
                exchangeArbi.Bid = Ticker.bid ?? 0.0;
                exchangeArbi.DayVolumeUSDT = Ticker.quoteVolume ?? 0.0;
                exchangeArbi.AskVolume = Ticker.askVolume ?? 0.0;
                exchangeArbi.BidVolume = Ticker.bidVolume ?? 0.0;
            }
            else
            {
                arbiList.Add(new Arbi()
                {
                    ExchangeObject = ExchangeObject,
                    FriendlySymbolName = Key,
                    FullSymbolName = Ticker.symbol ?? "",
                    Ask = Ticker.ask ?? 0.0,
                    Bid = Ticker.bid ?? 0.0,
                    AskVolume = Ticker.askVolume ?? 0.0,
                    BidVolume = Ticker.bidVolume ?? 0.0,
                    DayVolumeUSDT = Ticker.quoteVolume ?? 0.0
                });
            }
        }
    }
}
