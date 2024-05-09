using ArbiDataLib.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
            AskNetworks = other.AskNetworks;
            BidNetworks = other.BidNetworks;
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

        [JsonPropertyName("askNet")]
        public List<TokenNetworkResponse> AskNetworks { get; set; } = [];

        [JsonPropertyName("bidNet")]
        public List<TokenNetworkResponse> BidNetworks { get; set; } = [];

        public static readonly Dictionary<string, string[]> networkSynonyms = new()
        {
            { "ERC20", new string[] { "ERC-20", "ETH", "ERC 20" } },
            { "BEP20", new string[] { "BEP-20, BEP", "BSC", "ERC 20"} },
            { "SOL", new string[] {"SOLANA"} }
        };

        public static bool IsNetworkEqual(string first, string second)
        {
            first = first.ToUpper();
            second = second.ToUpper();
            // Going through all the synonyms of the first word
            foreach (var synonym in networkSynonyms.GetValueOrDefault(first, new string[0]))
            {
                // If the second word matches any synonym of the first word, return true
                if (synonym.Equals(second, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Going through all the synonyms of the second word
            foreach (var synonym in networkSynonyms.GetValueOrDefault(second, new string[0]))
            {
                // If the first word matches any synonym of the second word, return true
                if (synonym.Equals(first, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return first.Equals(second, StringComparison.OrdinalIgnoreCase);
        }

        public static IList<ArbiSameNetwork> HasSameNetworks(IList<TokenNetworkResponse> from, IList<TokenNetworkResponse> to)
        {
            IList<ArbiSameNetwork> res = [];
            foreach (var net1 in from)
            {
                foreach (var net2 in to)
                {
                    if (IsNetworkEqual(net1.Code, net2.Code))
                    {
                        if (net1.Active && net1.Withdraw
                            && net2.Active && net2.Deposit)
                        {
                            res.Add(new ArbiSameNetwork() { Ask = net1, Bid = net2 });
                        }
                    }
                }
            }
            return res;
        }
    }

    public class ArbiSameNetwork
    {
        public required TokenNetworkResponse Ask { get; set; }
        public required TokenNetworkResponse Bid { get; set; }
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
            string sellUrl,
            ArbiSameNetwork net) : base(arbi)
        {
            BuyName = buyName;
            BuyUrl = buyUrl;
            WithdrawUrl = withdrawUrl;
            SellName = sellName;
            DepositUrl = depositUrl;
            SellUrl = sellUrl;
            BestNetwork = net;
        }

        public string BuyName { get; set; } = string.Empty;
        public string BuyUrl { get; set; } = string.Empty;
        public string WithdrawUrl { get; set;} = string.Empty;
        public string SellName { get; set; } = string.Empty;
        public string DepositUrl { get; set;} = string.Empty;
        public string SellUrl { get; set;} = string.Empty;

        public ArbiSameNetwork? BestNetwork { get; set; } = null;

        public double CalculateProfit(double balance)
        {
            double boughtAmount = balance / Ask;
            double sold = boughtAmount * Bid;
            double feeUsdt = BestNetwork?.Ask.Fee * Ask ?? 0.0;
            double afterFee = sold - feeUsdt;
            return afterFee - balance;
        }
    }
}
