using ArbiLib.Models;
using ccxt;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
namespace ArbiLib.Services
{
    public class ArbiService(List<Exchange> InExchanges) : IDisposable
    {
        public ConcurrentDictionary<string, ConcurrentBag<Arbi>> ArbiDictionary { get; private set; } = [];
        public List<Exchange> Exchanges { get; private set; } = InExchanges;
        public List<ExchangeWorker> Workers { get; private set; } = [];

        public ConcurrentQueue<ArbiOportunity> ArbiOportunitiesQueue { get; private set; } = [];
        public List<ArbiOportunity> Oportunities { get; private set; } = [];
        private int _maxOportunities = 50;
        public int MaxOportunities 
        { 
            get => _maxOportunities;
            set 
            {
                _maxOportunities = value;
                Oportunities.Clear();
            } 
        }

        public double MinProfitPercent { get; set; } = 1.0;
        public double MaxProfitPercent { get; set; } = 50.0;
        public double MinAskVolumeUsdt { get; set; } = 500.0;
        public double MinBidVolumeUsdt { get; set; } = 500.0;

        private CancellationTokenSource _cancellationTokenSource = new();
        private Task? _collectArbiTask = null;
        private Task? _processArbiTask = null;
        public void Dispose()
        {
            foreach (var ex in Workers) 
            { 
                ex.Dispose();
            }
            _cancellationTokenSource.Cancel();
            try
            {
                _collectArbiTask?.Wait();
            }
            catch (Exception) { }
            try
            {
                _processArbiTask?.Wait();
            }
            catch (Exception) { }
            _cancellationTokenSource.Dispose();

            ArbiDictionary.Clear(); 
            Exchanges.Clear();
            Workers.Clear();
        }

        public void StartWorkers()
        {
            foreach (var ex in Exchanges)
            {
                ExchangeWorker worker = new ExchangeWorker(ex, this);
                Workers.Add(worker);
                worker.StartWork();
            }
            _collectArbiTask = CollectArbi();
            _processArbiTask = ProcessArbi();
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

        public async Task ProcessArbi()
        {
            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (ArbiOportunitiesQueue.TryDequeue(out ArbiOportunity? value))
                    {
                        var res = Oportunities.FirstOrDefault(x => x.MinimalAsk?.Ticker == value.MinimalAsk?.Ticker);
                        if(res is not null)
                        {
                            res.MinimalAsk = value.MinimalAsk;
                            res.MaximalBid = value.MaximalBid;
                        }
                        else
                        {
                            Oportunities.Add(value);
                        }
                        Oportunities = Oportunities.OrderByDescending(x => x.PercentDiff()).Take(MaxOportunities).ToList();
                    }
                    await Task.Delay(250);
                }
            });
        }



        public async Task CollectArbi()
        {
            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Parallel.ForEach(ArbiDictionary, item =>
                    {
                        ConcurrentBag<Arbi> symbolDatas = item.Value;
                        // Find the element with the lowest ask
                        Arbi? lowestAskElement = symbolDatas.OrderBy(arbi => arbi.Ask).FirstOrDefault();
                        if (lowestAskElement is null)
                        {
                            return;
                        }
                        // Find the element with the highest bid
                        Arbi? highestBidElement = symbolDatas
                            .Where(arbi => arbi.ExchangeObject != lowestAskElement.ExchangeObject)
                            .Where(arbi => lowestAskElement?.Ask < arbi.Bid)
                            .OrderByDescending(arbi => arbi.Bid)
                            .FirstOrDefault();
                        if (highestBidElement is null)
                        {
                            return;
                        }
                        var oportunity = new ArbiOportunity()
                        {
                            MinimalAsk = lowestAskElement,
                            MaximalBid = highestBidElement
                        };
                        double diff = oportunity.PercentDiff();
                        if (diff > 0 && diff > MinProfitPercent && diff <= MaxProfitPercent
                            && lowestAskElement.AskVolumeUsdt > MinAskVolumeUsdt 
                            && highestBidElement.BidVolumeUsdt > MinBidVolumeUsdt)
                        {
                            ArbiOportunitiesQueue.Enqueue(oportunity);
                        }
                    });
                    await Task.Delay(250);
                }
            });
        }

        static int FindIndexToInsert(List<ArbiOportunity> List, ArbiOportunity NewElement)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (List[i].PercentDiff() <= NewElement.PercentDiff())
                {
                    return i;
                }
            }
            return List.Count;
        }
    }
}
