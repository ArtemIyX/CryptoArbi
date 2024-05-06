
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ArbiDataLib.Data
{
    public partial class ArbiFilter
    {
        [JsonPropertyName("price")]
        public double MinPrice { get; set; } = 0.0;

        [JsonPropertyName("vol")]
        public double MinVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("dayVol")]
        public double MinDayVolumeUSDT { get; set; } = 0.0;

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

        public static string[] MakeForbidden(string input) => string.IsNullOrEmpty(input) ? [] : input.Split(',');
        
        public string[] MakeForbiddenBuy() => ArbiFilter.MakeForbidden(ForbiddenBuy);
        public string[] MakeForbiddenSell() => ArbiFilter.MakeForbidden(ForbiddenSell);

        public static bool IsValidCommaSeparatedString(string input) => ForbiddenRegex().IsMatch(input);

        public bool IsValidForbiddenBuy() => !string.IsNullOrEmpty(ForbiddenBuy) && IsValidCommaSeparatedString(ForbiddenBuy);
        public bool IsValidForbiddenSell() => !string.IsNullOrEmpty(ForbiddenSell) && IsValidCommaSeparatedString(ForbiddenSell);

        [GeneratedRegex(@"^[a-zA-Z,]+$")]
        public static partial Regex ForbiddenRegex();

        public IDictionary<string, string?> ToArgsDictionary()
        {
            IDictionary<string, string?> dictionary = new Dictionary<string, string?>()
            {
                {"price", MinPrice.ToString(CultureInfo.InvariantCulture)},
                {"vol", MinVolumeUSDT.ToString(CultureInfo.InvariantCulture)},
                {"dayVol", MinDayVolumeUSDT.ToString(CultureInfo.InvariantCulture)},
                {"minP", MinPercent.ToString(CultureInfo.InvariantCulture)},
                {"maxP", MaxPercent.ToString(CultureInfo.InvariantCulture)},
                {"num", Amount.ToString(CultureInfo.InvariantCulture)}
            };
            IDictionary<string, string?> res = dictionary;
            if(!string.IsNullOrEmpty(ForbiddenBuy))
                res.Add("buy", ForbiddenBuy);
            if(!string.IsNullOrEmpty(ForbiddenSell))
                res.Add("sell", ForbiddenSell);

            return res;
        }
    }
}
