using ccxt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiLib.Models
{
    public class Arbi
    {
        public required string FullSymbolName { get; set; }
        public required string FriendlySymbolName { get; set; }
        public required ccxt.Exchange ExchangeObject { get; set; }
        public double Ask { get; set; }
        public double Bid { get; set; }
        public double DayVolumeUSDT { get; set; }
        public double AskVolume { get; set; }
        public double BidVolume { get; set; }

        public double AskVolumeUsdt => AskVolume * Ask;
        public double BidVolumeUsdt => BidVolume * Bid;

        public string DayVolumeStr => MakeVolumeString(DayVolumeUSDT);
        public string AskVolumeStr => MakeVolumeString(AskVolume);
        public string BidVolumeStr => MakeVolumeString(BidVolume);

        public string AskVolumeUsdtStr => MakeVolumeString(AskVolumeUsdt);
        public string BidVolumeUsdtStr => MakeVolumeString(BidVolumeUsdt);

        public override string ToString()
        {
            return $"{ExchangeObject.name} {FriendlySymbolName} - Ask: {Ask:F10}, Bid: {Bid:F10} (Vol: {DayVolumeStr})";
        }
        protected string MakeVolumeString(double Volume) => Volume >= 1_000_000 ? $"{Volume / 1_000_000:F2}kk" : Volume >= 1_000 ? $"{Volume / 1_000:F2}k" : Volume.ToString("F2");
    }
}
