
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ArbiDataLib.Data
{
    public partial class ArbiFilter
    {
        [JsonPropertyName("price")]
        public double MinPrice { get; set; } = 0.0;

        [JsonPropertyName("askVol")]
        public double MinAskVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("bidVol")]
        public double MinBidVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("askDayVol")]
        public double MinAskDayVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("bidDayVol")]
        public double MinBidDayVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("minP")]
        public double MinPercent { get; set; } = 0.0;

        [JsonPropertyName("maxP")]
        public double MaxPercent { get; set; } = 100.0;

        [JsonPropertyName("num")]
        public int Amount { get; set; } = 50;

        [JsonPropertyName("buy")]
        public string ForbiddenBuy { get; set; } = string.Empty;

        [JsonPropertyName("sell")]
        public string ForbiddenSell { get; set; } = string.Empty;

        private static string[] MakeForbidden(string input) => string.IsNullOrEmpty(input) ? [] : input.Split(',');
       
        public string[] MakerForbiddenBuy() => ArbiFilter.MakeForbidden(ForbiddenBuy);

        public string[] MakerForbiddenSell() => ArbiFilter.MakeForbidden(ForbiddenSell);

        public static bool IsValidCommaSeparatedString(string input) => ForbiddenRegex().IsMatch(input);

        public bool IsValidForbiddenBuy() => !string.IsNullOrEmpty(ForbiddenBuy) && IsValidCommaSeparatedString(ForbiddenBuy);
        public bool IsValidForbiddenSell() => !string.IsNullOrEmpty(ForbiddenSell) && IsValidCommaSeparatedString(ForbiddenSell);

        [GeneratedRegex(@"^[a-zA-Z,]+$")]
        public static partial Regex ForbiddenRegex();
    }
}
