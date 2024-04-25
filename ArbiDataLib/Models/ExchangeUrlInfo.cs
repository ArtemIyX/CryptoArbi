using System.Text.Json.Serialization;


namespace ArbiDataLib.Models
{
    public class ExchangeUrlInfo
    {
        [JsonPropertyName("id")]
        public string ExchangeId { get; set; } = string.Empty;

        [JsonPropertyName("home")]
        public string HomeURL { get; set; } = string.Empty;

        [JsonPropertyName("trade")]
        public string TradeURL { get; set; } = string.Empty;

        [JsonPropertyName("deposit")]
        public string DepositURL { get; set; } = string.Empty;

        [JsonPropertyName("withdraw")]
        public string WithdrawalURL { get; set; } = string.Empty;
    }
}
