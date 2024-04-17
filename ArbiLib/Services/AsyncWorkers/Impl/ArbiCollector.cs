using ArbiLib.Models;
using ArbiLib.Services.Worker;
using System.Collections.Concurrent;

namespace ArbiLib.Services.AsyncWorkers.Impl
{
    public class ArbiCollector(ArbiService InService) : AsyncWorker(InService)
    {
        protected override async Task DoWork()
        {
            Parallel.ForEach(Arbi.ArbiDictionary, item =>
            {
                ConcurrentBag<Arbi> symbolDatas = item.Value;
                // Find the element with the lowest ask
                Arbi? lowestAskElement = symbolDatas.OrderBy(arbi => arbi.Ask).FirstOrDefault();
                if (lowestAskElement is null) return;

                // Find the element with the highest bid
                Arbi? highestBidElement = symbolDatas
                    .Where(arbi => arbi.ExchangeObject != lowestAskElement.ExchangeObject)
                    .Where(arbi => lowestAskElement?.Ask < arbi.Bid)
                    .OrderByDescending(arbi => arbi.Bid)
                    .FirstOrDefault();

                if (highestBidElement is null) return;
                ArbiOportunity oportunity = new ArbiOportunity()
                {
                    MinimalAsk = lowestAskElement,
                    MaximalBid = highestBidElement
                };

                double diff = oportunity.PercentDiff();
                if (diff > 0 && diff > Arbi.MinProfitPercent && diff <= Arbi.MaxProfitPercent
                    && lowestAskElement.AskVolumeUsdt > Arbi.MinAskVolumeUsdt
                    && highestBidElement.BidVolumeUsdt > Arbi.MinBidVolumeUsdt
                    && lowestAskElement.DayVolumeUSDT > Arbi.MinDayVolumeUsdt
                    && highestBidElement.DayVolumeUSDT > Arbi.MinDayVolumeUsdt)
                {
                    Arbi.ArbiOportunitiesQueue.Enqueue(oportunity);
                }
            });
            await Task.Delay(250);

        }
    }
}
