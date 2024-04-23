using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ArbiDataLib.Models
{
    [Table("Tokens")]
    public class ExchangeToken
    {
        [Key, Required, NotNull]
        public long Id { get; set; }

        [Required, NotNull]
        public required string FullSymbolName { get; set; }

        [Required, NotNull]
        public required string DisplayName { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? Ask { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? Bid { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? DayVolumeUSDT { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? AskVolume { get; set; }

        [MaybeNull, DefaultValue(null)]
        public double? BidVolume { get; set; }

        public DateTime Updated { get; set; }

        public required string ExchangeId { get; set; }
        public virtual ExchangeEntity? Exchange { get; set; }

        public double? AskVolumeUsdt => AskVolume is not null ? AskVolume * Ask : null;
        public double? BidVolumeUsdt => AskVolume is not null ? BidVolume * Bid : null;

        public ExchangeTokenResponse ToResponse() =>
            new ExchangeTokenResponse
            {
                Id = this.Id,
                FullSymbolName = this.FullSymbolName,
                DisplayName = this.DisplayName,
                Ask = this.Ask,
                Bid = this.Bid,
                DayVolumeUSDT = this.DayVolumeUSDT,
                AskVolume = this.AskVolume,
                BidVolume = this.BidVolume,
                Updated = this.Updated,
                ExchangeId = this.ExchangeId
            };
    }

    public class ExchangeTokenResponse()
    {
        [JsonPropertyName("id")]
        public long Id { get; set; } = 0;

        [JsonPropertyName("fullSymbolName")]
        public string FullSymbolName { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("ask")]
        public double? Ask { get; set; } = null;

        [JsonPropertyName("bid")]
        public double? Bid { get; set; } = null;

        [JsonPropertyName("dayVolumeUSDT")]
        public double? DayVolumeUSDT { get; set; } = null;

        [JsonPropertyName("askVolume")]
        public double? AskVolume { get; set; } = null;

        [JsonPropertyName("bidVolume")]
        public double? BidVolume { get; set; } = null;

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; } = DateTime.MinValue;

        [JsonPropertyName("exchangeId")]
        public string ExchangeId { get; set; } = string.Empty;
    }
}
