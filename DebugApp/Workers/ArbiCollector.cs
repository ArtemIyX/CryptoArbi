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
                ArbiOportunity entity = new ArbiOportunity()
                {
                    MinimalAsk = lowestAskElement,
                    MaximalBid = highestBidElement
                };

                
                double diff = entity.PercentDiff();
                if (diff >= 0
                    && diff >= ArbiService.MinProfitPercent
                    && diff <= ArbiService.MaxProfitPercent
                    && entity.MinimalAsk.AskVolumeUsdt > ArbiService.MinAskVolumeUsdt
                    && entity.MaximalBid.BidVolumeUsdt > ArbiService.MinBidVolumeUsdt
                    && entity.MinimalAsk.DayVolumeUSDT > ArbiService.MinDayVolumeUsdt
                    && (entity.MaximalBid?.DayVolumeUSDT ?? 0) > ArbiService.MinDayVolumeUsdt)
                {
                    ArbiService.ArbiOportunitiesQueue.Enqueue(entity);
                }
            });
            await Task.Delay(250);

        }
    }
}
