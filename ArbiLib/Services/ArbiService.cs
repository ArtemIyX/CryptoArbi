using ArbiLib.Models;
using ccxt;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
namespace ArbiLib.Services
{
    public class ArbiService(List<Exchange> InExchanges) : IDisposable
    {
        public ConcurrentDictionary<string, List<Arbi>> ArbiDictionary { get; private set; } = [];
        public List<Exchange> Exchanges { get; private set; } = InExchanges;
        public List<ExchangeWorker> Workers { get; private set; } = [];
        public List<ArbiOportunity> Oportunities { get; private set; } = [];
        public Queue<ArbiOportunity> ArbiOportunitiesQueue { get; private set; } = [];
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

        public void UpdateTicker(string Key, ccxt.Exchange ExchangeObject, double Ask, double Bid, string Symbol)
        {
            List<Arbi> arbiList = ArbiDictionary.GetOrAdd(Key, _ => new List<Arbi>());
            Arbi? exchangeArbi = arbiList.FirstOrDefault(x => x.ExchangeObject == ExchangeObject);
            if (exchangeArbi is not null)
            {
                exchangeArbi.Ask = Ask;
                exchangeArbi.Bid = Bid;
            }
            else
            {
                arbiList.Add(new Arbi(ExchangeObject, Ask, Bid, Symbol));
            }
        }

        public async Task ProcessArbi()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                if(ArbiOportunitiesQueue.TryDequeue(out ArbiOportunity? value))
                {
                    
                }
                await Task.Delay(100);
            }
        }
        public async Task CollectArbi()
        {
            while(!_cancellationTokenSource.IsCancellationRequested)
            {
                Parallel.ForEach(ArbiDictionary, item =>
                {
                    List<Arbi> symbolDatas = item.Value;
                    // Find the element with the lowest ask
                    Arbi? lowestAskElement = symbolDatas.OrderBy(arbi => arbi.Ask).FirstOrDefault();
                    if (lowestAskElement is null)
                    {
                        return;
                    }
                    // Find the element with the highest bid
                    Arbi? highestBidElement = symbolDatas
                        .Where(arbi => arbi.ExchangeObject != lowestAskElement?.ExchangeObject && lowestAskElement?.Ask < arbi.Bid)
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
                    ArbiOportunitiesQueue.Enqueue(oportunity);
                });
                await Task.Delay(100);
            }
           
        }

        public ArbiOportunity? FindOportunity(string Key)
        {
            List<Arbi> list = GetArbi(Key);
            if (list.Count == 0)
            {
                return null;
            }
            // Find the element with the lowest ask
            Arbi? lowestAskElement = list.OrderBy(arbi => arbi.Ask).FirstOrDefault();

            // Find the element with the highest bid
            Arbi? highestBidElement = list.Where(arbi => arbi.ExchangeObject != lowestAskElement?.ExchangeObject).OrderByDescending(arbi => arbi.Bid).FirstOrDefault();

            if (lowestAskElement != null && highestBidElement != null)
            {
                return new ArbiOportunity()
                {
                    MinimalAsk = lowestAskElement,
                    MaximalBid = highestBidElement
                };
            }
            return null;
        }

        public List<Arbi> GetArbi(string Key)
        {
            if (!ArbiDictionary.ContainsKey(Key))
            {
                return new List<Arbi>();
            }
            return ArbiDictionary[Key];
        }


    }
}
