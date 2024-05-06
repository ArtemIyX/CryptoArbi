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
            new(
                Id,
                FullSymbolName,
                DisplayName,
                Ask,
                Bid,
                DayVolumeUSDT,
                AskVolume,
                BidVolume,
                Updated,
                ExchangeId
            );
    }

    public class ExchangeTokenResponse
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

        // Empty constructor
        public ExchangeTokenResponse()
        {
        }

        // Full constructor
        public ExchangeTokenResponse(long id, string fullSymbolName,
            string displayName, double? ask, double? bid, double? dayVolumeUSDT,
            double? askVolume, double? bidVolume, DateTime updated, string exchangeId)
        {
            Id = id;
            FullSymbolName = fullSymbolName;
            DisplayName = displayName;
            Ask = ask;
            Bid = bid;
            DayVolumeUSDT = dayVolumeUSDT;
            AskVolume = askVolume;
            BidVolume = bidVolume;
            Updated = updated;
            ExchangeId = exchangeId;
        }
        // Copy constructor
        public ExchangeTokenResponse(ExchangeTokenResponse other)
        {
            Id = other.Id;
            FullSymbolName = other.FullSymbolName;
            DisplayName = other.DisplayName;
            Ask = other.Ask;
            Bid = other.Bid;
            DayVolumeUSDT = other.DayVolumeUSDT;
            AskVolume = other.AskVolume;
            BidVolume = other.BidVolume;
            Updated = other.Updated;
            ExchangeId = other.ExchangeId;
        }
    }

    public class ExchangeTokenVisual : ExchangeTokenResponse
    {
        public ExchangeTokenVisual() : base() { }

        public ExchangeTokenVisual(ExchangeTokenResponse item,
            string exchangeName, string tradeUrl, string depositUrl, string withdrawUrl) : base(item)
        {
            ExchangeName = exchangeName;
            TradeUrl = tradeUrl;
            WithdrawUrl = withdrawUrl;
            DepositUrl = depositUrl;
        }

        public string ExchangeName { get; set; } = string.Empty;
        public string TradeUrl { get; set; } = string.Empty;
        public string DepositUrl { get; set; } = string.Empty;
        public string WithdrawUrl { get; set; } = string.Empty;
    }
}
