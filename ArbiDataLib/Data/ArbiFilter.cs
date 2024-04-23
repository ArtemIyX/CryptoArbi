using System.Text.Json.Serialization;

namespace ArbiDataLib.Data
{
    public class ArbiFilter
    {
        [JsonPropertyName("minAsk")]
        public double MinAsk { get; set; } = 0.0;

        [JsonPropertyName("minBid")]
        public double MinBid { get; set; } = 0.0;

        [JsonPropertyName("minAskVolumeUSDT")]
        public double MinAskVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("minBidVolumeUSDT")]
        public double MinBidVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("minDayVolumeUSDT")]
        public double MinDayVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("minPercent")]
        public double MinPercent { get; set; } = 0.0;

        [JsonPropertyName("maxPercent")]
        public double MaxPercent { get; set; } = 100.0;

        [JsonPropertyName("amount")]
        public int Amount { get; set; } = 50;
    }
}
