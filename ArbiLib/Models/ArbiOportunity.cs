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
                return $"{PercentDiff():F2}%\t{MinimalAsk?.Ticker}\t{MinimalAsk?.ExchangeObject.name}\t{MinimalAsk?.Ask:F10}\t (AskVol: {MinimalAsk?.AskVolumeUsdtStr}$) \tB|" +
                    $"S (BidVol: {MaximalBid?.BidVolumeUsdtStr}$)\t{MaximalBid?.ExchangeObject.name}\t{MaximalBid?.Bid:F10}";
            }
            return "Invalid ArbiOportunity " + base.ToString();
        }


    }
}
