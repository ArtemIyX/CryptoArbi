using ArbiLib.Models;
using ArbiLib.Services.AsyncWorkers.Impl;
using ccxt;
using System.Collections.Concurrent;
using System.Diagnostics;
namespace ArbiLib.Services
{
    public class ArbiService : IDisposable
    {
        public ConcurrentDictionary<string, ConcurrentBag<Arbi>> ArbiDictionary { get; private set; } = [];
        public List<Exchange> Exchanges { get; private set; }
        public List<ExchangeWorker> ExchangeWorkers { get; private set; } = [];

        public ArbiCollector Collector { get; private set; }
        public ArbiProcessor Processor { get; private set; }

        public ConcurrentQueue<ArbiOportunity> ArbiOportunitiesQueue { get; private set; } = [];
        public List<ArbiOportunity> OportunityList { get; set; } = [];

        private int _maxOportunities = 50;
        public int MaxOportunities
        {
            get => _maxOportunities;
            set
            {
                _maxOportunities = value;
                OportunityList.Clear();
            }
        }

        public double MinProfitPercent { get; set; } = 1.0;
        public double MaxProfitPercent { get; set; } = 50.0;
        public double MinAskVolumeUsdt { get; set; } = 500.0;
        public double MinBidVolumeUsdt { get; set; } = 500.0;
        public double MinDayVolumeUsdt { get; set; } = 0;

        public ArbiService(List<Exchange> InExchanges)
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
                ExchangeWorker worker = new ExchangeWorker(ex, this);
                ExchangeWorkers.Add(worker);
                worker.StartWork();
            }
            Collector.StartWork();
            Processor.StartWork();
        }

        public void UpdateTicker(string Key, ccxt.Exchange ExchangeObject, double Ask, double Bid, string Symbol,
             double DayVolume, double AskVolume, double BidVolume)
        {
            ConcurrentBag<Arbi> arbiList = ArbiDictionary.GetOrAdd(Key, _ => new ConcurrentBag<Arbi>());
            Arbi? exchangeArbi = arbiList.FirstOrDefault(x => x.ExchangeObject == ExchangeObject);
            if (exchangeArbi is not null)
            {
                exchangeArbi.Ask = Ask;
                exchangeArbi.Bid = Bid;
                exchangeArbi.DayVolumeUSDT = DayVolume;
                exchangeArbi.AskVolume = AskVolume;
                exchangeArbi.BidVolune = BidVolume;
            }
            else
            {
                arbiList.Add(new Arbi()
                {
                    ExchangeObject = ExchangeObject,
                    Ask = Ask,
                    Bid = Bid,
                    Ticker = Symbol,
                    AskVolume = AskVolume,
                    BidVolune = BidVolume,
                    DayVolumeUSDT = DayVolume
                });
            }
        }
    }
}
