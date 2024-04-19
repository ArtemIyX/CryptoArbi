using ArbiLib.Models;
using System.Collections.Concurrent;

namespace DebugApp.Workers
{
    public class ArbiCollector(LocalArbiService InService) : LocalAsyncWorker(InService)
    {
        protected override async Task DoWork()
        {
            Parallel.ForEach(ArbiService.ArbiDictionary, async item =>
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


                await HandleOportunity(oportunity);
                ArbiService.ArbiOportunitiesQueue.Enqueue(oportunity);
            });
            await Task.Delay(250);

        }

        protected virtual async Task HandleOportunity(ArbiOportunity entity)
        {
            double diff = entity.PercentDiff();
            if (diff < 0)
            {
                return;
            }
            if (diff < ArbiService.MinProfitPercent)
            {
                return;
            }
            if (diff > ArbiService.MaxProfitPercent)
            {
                return;
            }


            if (entity.MinimalAsk.AskVolumeUsdt > ArbiService.MinAskVolumeUsdt
               && entity.MaximalBid.BidVolumeUsdt > ArbiService.MinBidVolumeUsdt
               && entity.MinimalAsk.DayVolumeUSDT > ArbiService.MinDayVolumeUsdt
               && entity.MaximalBid?.DayVolumeUSDT > ArbiService.MinDayVolumeUsdt)
            {
                ArbiService.ArbiOportunitiesQueue.Enqueue(entity);
            }
        }
    }
}
