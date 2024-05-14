using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArbiDataLib.Data
{
    public class ExchangeApiInfo
    {
        [JsonPropertyName("id")]
        public string ExchangeId { get; set; } = string.Empty;

        [JsonPropertyName("api")]
        public string Api { get; set; } = string.Empty;

        [JsonPropertyName("secret")]
        public string Secret { get; set; } = string.Empty;
    }
}
