namespace ArbiReader.Data
{
    public class ArbiFilter
    {
/*        public IList<string> AllowedBuy { get; set; } = [];
        public IList<string> AllowedSell { get; set; } = [];*/

        public double MinAsk { get; set; } = 0.0;
        public double MinBid { get; set; } = 0.0;

        public double MinAskVolumeUsdt { get; set; } = 0.0;
        public double MinBidVolumeUsdt { get; set; } = 0.0;

        public double MinDayVolumeUsdt { get; set; } = 0.0;

        public double MinPercent { get; set; } = 0.0;
        public double MaxPercent { get; set; } = 100.0;

        public int Amount { get; set; } = 15;
    }
}
