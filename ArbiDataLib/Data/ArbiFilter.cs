
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ArbiDataLib.Data
{
    public partial class ArbiFilter
    {
        [BindProperty(Name = "ask")]
        [JsonPropertyName("ask")]
        public double MinAsk { get; set; } = 0.0;

        [BindProperty(Name = "bid")]
        [JsonPropertyName("bid")]
        public double MinBid { get; set; } = 0.0;

        [BindProperty(Name = "askVol")]
        [JsonPropertyName("askVol")]
        public double MinAskVolumeUSDT { get; set; } = 0.0;

        [BindProperty(Name = "bidVol")]
        [JsonPropertyName("bidVol")]
        public double MinBidVolumeUSDT { get; set; } = 0.0;

        [BindProperty(Name = "dayVol")]
        [JsonPropertyName("dayVol")]
        public double MinDayVolumeUSDT { get; set; } = 0.0;

        [BindProperty(Name = "minP")]
        [JsonPropertyName("minP")]
        public double MinPercent { get; set; } = 0.0;

        [BindProperty(Name = "maxP")]
        [JsonPropertyName("maxP")]
        public double MaxPercent { get; set; } = 100.0;

        [BindProperty(Name = "num")]
        [JsonPropertyName("num")]
        public int Amount { get; set; } = 50;

        [BindProperty(Name = "buy")]
        [JsonPropertyName("buy")]
        public string ForbiddenBuy { get; set; } = string.Empty;

        [BindProperty(Name = "sell")]
        [JsonPropertyName("sell")]
        public string ForbiddenSell { get; set; } = string.Empty;

        private static string[] MakeForbidden(string input) => string.IsNullOrEmpty(input) ? [] : input.Split(',');
       
        public string[] MakerForbiddenBuy() => ArbiFilter.MakeForbidden(ForbiddenBuy);

        public string[] MakerForbiddenSell() => ArbiFilter.MakeForbidden(ForbiddenSell);

        public static bool IsValidCommaSeparatedString(string input) => ForbiddenRegex().IsMatch(input);

        public bool IsValidForbiddenBuy() => !string.IsNullOrEmpty(ForbiddenBuy) && IsValidCommaSeparatedString(ForbiddenBuy);
        public bool IsValidForbiddenSell() => !string.IsNullOrEmpty(ForbiddenSell) && IsValidCommaSeparatedString(ForbiddenSell);

        [GeneratedRegex(@"^[a-zA-Z0-9,]+$")]
        public static partial Regex ForbiddenRegex();
    }
}
