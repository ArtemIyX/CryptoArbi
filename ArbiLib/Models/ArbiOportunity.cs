using ArbiLib.Libs;
using System.Globalization;

namespace ArbiLib.Models
{
    public class ArbiOportunity
    {
        public Arbi? MinimalAsk { get; set; }
        public Arbi? MaximalBid { get; set; }

        public bool IsValid => MinimalAsk != null && MaximalBid != null;

        public double PercentDiff()
        {
            double a = MinimalAsk?.Ask ?? 0;
            double b = MaximalBid?.Bid ?? 0;
            return TickerLib.CalculatePercentDifference(a, b);
        }

        public override string ToString()
        {
            if(IsValid)
            {
                return $"{MinimalAsk?.Ticker} {MinimalAsk?.ExchangeObject.name} {MinimalAsk?.Ask:F20} B|S {MaximalBid?.ExchangeObject.name} {MaximalBid?.Bid:F20} +{PercentDiff():F20}%";
            }
            return "Invalid ArbiOportunity " + base.ToString();
        }


    }
}
