using System;
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

        [NotNull, DefaultValue(false)]
        public bool Active { get; set; }

        [NotNull, DefaultValue(false)]
        public bool Deposit { get; set; }

        [NotNull, DefaultValue(false)]
        public bool Withdraw { get; set; }

        public string ExchangeId { get; set; }

        [ForeignKey("ExchangeId")]
        public virtual ExchangeEntity? Exchange { get; set; }

        public virtual ICollection<ExchangeTokenNetwork> Networks { get; set; }

        public virtual ICollection<OrderBookItem> OrderBook { get; set; }

        public double? AskVolumeUsdt => AskVolume is not null ? AskVolume * Ask : null;
        public double? BidVolumeUsdt => AskVolume is not null ? BidVolume * Bid : null;

        public ExchangeTokenResponse ToResponse() =>
            new(
                Id,
                DisplayName,
                Ask,
                Bid,
                DayVolumeUSDT,
                AskVolume,
                BidVolume,
                Updated,
                ExchangeId,
                Active,
                Deposit,
                Withdraw,
                Networks.Select(x => x.ToResponse()).ToList()
            );
    }

    public class ExchangeTokenResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; } = 0;


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

        [JsonPropertyName("active")]
        public bool Active { get; set; } = false;

        [JsonPropertyName("deposit")]
        public bool Deposit { get; set; } = false;

        [JsonPropertyName("withdraw")]
        public bool Withdraw { get; set; } = false;

        [JsonPropertyName("networks")]
        public ICollection<TokenNetworkResponse> Networks { get; set; } = [];

        // Empty constructor
        public ExchangeTokenResponse()
        {
        }

        // Full constructor
        public ExchangeTokenResponse(long id,
            string displayName, double? ask, double? bid, double? dayVolumeUSDT,
            double? askVolume, double? bidVolume, DateTime updated, string exchangeId,
            bool active, bool deposit, bool withdraw,
            ICollection<TokenNetworkResponse> networks)
        {
            Id = id;
            DisplayName = displayName;
            Ask = ask;
            Bid = bid;
            DayVolumeUSDT = dayVolumeUSDT;
            AskVolume = askVolume;
            BidVolume = bidVolume;
            Updated = updated;
            ExchangeId = exchangeId;
            Active = active;
            Deposit = deposit;
            Withdraw = withdraw;
            Networks = networks;
        }
        // Copy constructor
        public ExchangeTokenResponse(ExchangeTokenResponse other)
        {
            Id = other.Id;
            DisplayName = other.DisplayName;
            Ask = other.Ask;
            Bid = other.Bid;
            DayVolumeUSDT = other.DayVolumeUSDT;
            AskVolume = other.AskVolume;
            BidVolume = other.BidVolume;
            Updated = other.Updated;
            ExchangeId = other.ExchangeId;
            Active = other.Active;
            Deposit = other.Deposit;
            Withdraw = other.Withdraw;
            Networks = other.Networks;
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
