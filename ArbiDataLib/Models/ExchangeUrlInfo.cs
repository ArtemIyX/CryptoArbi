using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;


namespace ArbiDataLib.Models
{
    public class ExchangeUrlInfo
    {
        [BindProperty(Name = "id")]
        [JsonPropertyName("id")]
        public string ExchangeId { get; set; } = string.Empty;

        [BindProperty(Name = "home")]
        [JsonPropertyName("home")]
        public string HomeURL { get; set; } = string.Empty;

        [BindProperty(Name = "trade")]
        [JsonPropertyName("trade")]
        public string TradeURL { get; set; } = string.Empty;

        [BindProperty(Name = "deposit")]
        [JsonPropertyName("deposit")]
        public string DepositURL { get; set; } = string.Empty;

        [BindProperty(Name = "withdraw")]
        [JsonPropertyName("withdraw")]
        public string WithdrawalURL { get; set; } = string.Empty;
    }
}
