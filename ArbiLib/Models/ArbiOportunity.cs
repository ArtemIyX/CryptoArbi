using ArbiLib.Libs;
using System.Globalization;

namespace ArbiLib.Models
{
    public class ArbiOportunity
    {
        public Arbi? MinimalAsk { get; set; }
        public Arbi? MaximalBid { get; set; }

        public bool IsValid => MinimalAsk != null && MaximalBid != null;

        public override string ToString()
        {
            if(IsValid)
            {
                return $"{MinimalAsk?.Ticker} {MinimalAsk?.ExchangeObject.name} {MinimalAsk?.Ask:F20} B|S {MaximalBid?.ExchangeObject.name} {MaximalBid?.Bid:F20} +{PercentDiff():F20}%";
            }
            return "Invalid ArbiOportunity " + base.ToString();
        }

        public double PercentDiff() => TickerLib.CalculatePercentDifference(MinimalAsk.Ask, MaximalBid.Bid);
    }
}
