using System.Text.Json.Serialization;

namespace ArbiDataLib.Data
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
            AskVolumeUSDT = arbiItem.AskVolumeUSDT;
            BidId = arbiItem.BidId;
            Bid = arbiItem.Bid;
            BidVolume = arbiItem.BidVolume;
            BidVolumeUSDT = arbiItem.BidVolumeUSDT;
            DayVolumeUSDT = arbiItem.DayVolumeUSDT;
            PriceDifferencePercentage = arbiItem.PriceDifferencePercentage;
            Updated = arbiItem.Updated;
        }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("fullSymbolName")]
        public string FullSymbolName { get; set; } = string.Empty;

        [JsonPropertyName("exchangeId1")]
        public string ExchangeId1 { get; set; } = string.Empty;

        [JsonPropertyName("exchangeId2")]
        public string ExchangeId2 { get; set; } = string.Empty;

        [JsonPropertyName("askId")]
        public long AskId { get; set; } = 0;

        [JsonPropertyName("ask")]
        public double Ask { get; set; } = 0.0;

        [JsonPropertyName("askVolume")]
        public double AskVolume { get; set; } = 0.0;

        [JsonPropertyName("askVolumeUSDT")]
        public double AskVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("bidId")]
        public long BidId { get; set; } = 0;

        [JsonPropertyName("bid")]
        public double Bid { get; set; } = 0.0;

        [JsonPropertyName("bidVolume")]
        public double BidVolume { get; set; } = 0.0;

        [JsonPropertyName("bidVolumeUSDT")]
        public double BidVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("dayVolumeUSDT")]
        public double DayVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("priceDifferencePercentage")]
        public double PriceDifferencePercentage { get; set; } = 0.0;

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; } = DateTime.MinValue;
    }
}
