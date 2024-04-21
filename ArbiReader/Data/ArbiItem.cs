namespace ArbiReader.Data
{
    public class ArbiItem
    {
        public ArbiItem() { }
        public ArbiItem(ArbiItem arbiItem)
        {
            DisplayName = arbiItem.DisplayName;
            FullSymbolName = arbiItem.FullSymbolName;
            ExchangeId1 = arbiItem.ExchangeId1;
            ExchangeId2 = arbiItem.ExchangeId2;
            AskId = arbiItem.AskId;
            Ask = arbiItem.Ask;
            AskVolume = arbiItem.AskVolume;
            AskVolumeUsdt = arbiItem.AskVolumeUsdt;
            BidId = arbiItem.BidId;
            Bid = arbiItem.Bid;
            BidVolume = arbiItem.BidVolume;
            BidVolumeUsdt = arbiItem.BidVolumeUsdt;
            DayVolumeUSDT = arbiItem.DayVolumeUSDT;
            PriceDifferencePercentage = arbiItem.PriceDifferencePercentage;
            Updated = arbiItem.Updated;
        }

        public string DisplayName { get; set; } = string.Empty;
        public string FullSymbolName { get; set; } = string.Empty;
        public string ExchangeId1 { get; set; } = string.Empty;
        public string ExchangeId2 { get; set; } = string.Empty;
        public long AskId { get; set; } = 0;
        public double Ask { get; set; } = 0.0;
        public double AskVolume { get;set; } = 0.0;
        public double AskVolumeUsdt { get; set; } = 0.0;
        public long BidId { get; set; } = 0;
        public double Bid { get; set; } = 0.0;
        public double BidVolume { get; set; } = 0.0;
        public double BidVolumeUsdt { get; set; } = 0.0;
        public double DayVolumeUSDT { get; set; } = 0.0;
        public double PriceDifferencePercentage { get; set; } = 0.0;
        public DateTime Updated { get; set; } = DateTime.MinValue;
    }
}
