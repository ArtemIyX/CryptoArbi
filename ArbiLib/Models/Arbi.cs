using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiLib.Models
{
    public class Arbi
    {
        public Arbi(ccxt.Exchange ExchangeObject, double Ask, double Bid, string Symbol, 
            double Volume)
        {
            this.Ask = Ask;
            this.Bid = Bid;
            this.ExchangeObject = ExchangeObject;
            Ticker = Symbol;
            DayVolumeUSDT = Volume;
        }

        public string Ticker { get; set; }
        public ccxt.Exchange ExchangeObject { get; set; }
        public double Ask { get; set; }
        public double Bid { get; set; }
        public double DayVolumeUSDT { get; set; }
        
        public string DayVolumeStr => DayVolumeUSDT >= 1_000_000 ? $"{DayVolumeUSDT / 1_000_000:F2}kk" : DayVolumeUSDT >= 1_000 ? $"{DayVolumeUSDT / 1_000:F2}k" : DayVolumeUSDT.ToString("F2");

        public override string ToString()
        {
            return $"{ExchangeObject.name} {Ticker} - Ask: {Ask:F10}, Bid: {Bid:F10} (Vol: {DayVolumeStr})";
        }
    }
}
