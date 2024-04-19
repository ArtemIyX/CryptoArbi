using ArbiLib.Models;
using ArbiLib.Services.Worker;

namespace DebugApp.Workers
{
    public class ArbiProcessor(LocalArbiService InService) : LocalAsyncWorker(InService)
    {
        protected override async Task DoWork()
        {
            if (ArbiService.ArbiOportunitiesQueue.TryDequeue(out ArbiOportunity? value))
            {
                ArbiOportunity? res = ArbiService.OportunityList.FirstOrDefault(x => x.MinimalAsk?.FriendlySymbolName 
                    == value.MinimalAsk?.FriendlySymbolName);
                if (res is not null)
                {
                    res.MinimalAsk = value.MinimalAsk;
                    res.MaximalBid = value.MaximalBid;
                }
                else
                {
                    ArbiService.OportunityList.Add(value);
                }
            }
            await Task.Delay(50);
        }
    }
}
