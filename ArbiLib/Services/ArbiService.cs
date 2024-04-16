using ArbiLib.Models;
using ccxt;

namespace ArbiLib.Services
{
    public class ArbiService
    {
        protected Dictionary<string, List<Arbi>> ArbiDictionary { get; private set; }
        protected List<Exchange> Exchanges { get; private set; }

        public ArbiService(List<Exchange> InExchanges)
        {
            this.ArbiDictionary = new Dictionary<string, List<Arbi>>();
        }


        public void AddArbi(string Key, Arbi NewValue)
        {
            if (!ArbiDictionary.ContainsKey(Key))
            {
                ArbiDictionary.Add(Key, new List<Arbi>());
            }
            List<Arbi> arbiList = ArbiDictionary[Key];
            Arbi? exchangeArbi = arbiList.FirstOrDefault(x => x.ExchangeObject == NewValue.ExchangeObject);
            if (exchangeArbi is not null)
            {
                exchangeArbi.Ask = NewValue.Ask;
                exchangeArbi.Bid = NewValue.Bid;
            }
            else
            {
                arbiList.Add(NewValue);
            }
        }

        public ArbiOportunity[] FindOpportunities(double MaxPerecnt = 10.0, int Size = 50)
        {
            List<ArbiOportunity> res = ArbiDictionary.Keys
                .Select(key => FindOportunity(key))
                .Where(opportunity => opportunity != null)
                .ToList();

            return res
                 .Where(x => x.PercentDiff() <= MaxPerecnt) // Filter elements with percentage difference <= N
                 .OrderByDescending(x => x.PercentDiff())
                 .Take(Size)
                 .ToArray();
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
