﻿using System.Text.Json.Serialization;

namespace ArbiDataLib.Data
{
    public class ArbiItem
    {
        public ArbiItem() { }
        // Copy Constructor
        public ArbiItem(ArbiItem other)
        {
            DisplayName = other.DisplayName;
            FullSymbolName = other.FullSymbolName;
            ExchangeId1 = other.ExchangeId1;
            ExchangeId2 = other.ExchangeId2;
            AskId = other.AskId;
            Ask = other.Ask;
            AskVolume = other.AskVolume;
            AskVolumeUSDT = other.AskVolumeUSDT;
            AskDayVolumeUSDT = other.AskDayVolumeUSDT;
            BidId = other.BidId;
            Bid = other.Bid;
            BidVolume = other.BidVolume;
            BidVolumeUSDT = other.BidVolumeUSDT;
            BidDayVolumeUSDT = other.BidDayVolumeUSDT;
            PriceDifferencePercentage = other.PriceDifferencePercentage;
            Updated = other.Updated;
        }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullSymbolName { get; set; } = string.Empty;

        [JsonPropertyName("askExId")]
        public string ExchangeId1 { get; set; } = string.Empty;

        [JsonPropertyName("bidExId")]
        public string ExchangeId2 { get; set; } = string.Empty;

        [JsonPropertyName("askId")]
        public long AskId { get; set; } = 0;

        [JsonPropertyName("ask")]
        public double Ask { get; set; } = 0.0;

        [JsonPropertyName("askVol")]
        public double AskVolume { get; set; } = 0.0;

        [JsonPropertyName("askVolUSDT")]
        public double AskVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("askDayVolUSDT")]
        public double AskDayVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("bidId")]
        public long BidId { get; set; } = 0;

        [JsonPropertyName("bid")]
        public double Bid { get; set; } = 0.0;

        [JsonPropertyName("bidVol")]
        public double BidVolume { get; set; } = 0.0;

        [JsonPropertyName("bidVolUSDT")]
        public double BidVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("bidDayVolUSDT")]
        public double BidDayVolumeUSDT { get; set; } = 0.0;

        [JsonPropertyName("diff")]
        public double PriceDifferencePercentage { get; set; } = 0.0;

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; } = DateTime.MinValue;

    }

    public class ArbiItemVisual : ArbiItem
    {
        public ArbiItemVisual() : base()
        { 

        }

        public ArbiItemVisual(ArbiItem arbi,
            string buyName, 
            string buyUrl, 
            string withdrawUrl, 
            string sellName, 
            string depositUrl,
            string sellUrl) : base(arbi)
        {
            BuyName = buyName;
            BuyUrl = buyUrl;
            WithdrawUrl = withdrawUrl;
            SellName = sellName;
            DepositUrl = depositUrl;
            SellUrl = sellUrl;
        }

        public string BuyName { get; set; } = string.Empty;
        public string BuyUrl { get; set; } = string.Empty;
        public string WithdrawUrl { get; set;} = string.Empty;
        public string SellName { get; set; } = string.Empty;
        public string DepositUrl { get; set;} = string.Empty;
        public string SellUrl { get; set;} = string.Empty;
    }
}
