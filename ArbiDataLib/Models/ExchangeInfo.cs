using System.Text.Json.Serialization;


namespace ArbiDataLib.Models
{
    public class ExchangeInfo
    {
        [JsonPropertyName("ExchangeId")]
        public string ExchangeId { get; set; } = string.Empty;

        [JsonPropertyName("HomeURL")]
        public string HomeURL { get; set; } = string.Empty;

        [JsonPropertyName("TradeURL")]
        public string TradeURL { get; set; } = string.Empty;

        [JsonPropertyName("DepositURL")]
        public string DepositURL { get; set; } = string.Empty;

        [JsonPropertyName("WithdrawalURL")]
        public string WithdrawalURL { get; set; } = string.Empty;
    }
}
