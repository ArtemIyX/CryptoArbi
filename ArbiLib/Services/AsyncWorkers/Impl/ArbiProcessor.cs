using ArbiLib.Models;
using ArbiLib.Services.Worker;

namespace ArbiLib.Services.AsyncWorkers.Impl
{
    public class ArbiProcessor(ArbiService InService) : AsyncWorker(InService)
    {
        protected override async Task DoWork()
        {
            if (Arbi.ArbiOportunitiesQueue.TryDequeue(out ArbiOportunity? value))
            {
                ArbiOportunity? res = Arbi.OportunityList.FirstOrDefault(x => x.MinimalAsk?.Ticker == value.MinimalAsk?.Ticker);
                if (res is not null)
                {
                    res.MinimalAsk = value.MinimalAsk;
                    res.MaximalBid = value.MaximalBid;
                }
                else
                {
                    Arbi.OportunityList.Add(value);
                }
                Arbi.OportunityList = Arbi.OportunityList.OrderByDescending(x => x.PercentDiff()).Take(Arbi.MaxOportunities).ToList();
            }
            await Task.Delay(250);
        }
    }
}
