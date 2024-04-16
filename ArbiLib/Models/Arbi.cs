using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiLib.Models
{
    public class Arbi
    {
        public Arbi(ccxt.Exchange ExchangeObject, double Ask, double Bid, string ticker)
        {
            this.Ask = Ask;
            this.Bid = Bid;
            this.ExchangeObject = ExchangeObject;
            Ticker = ticker;
        }

        public string Ticker { get; set; }
        public ccxt.Exchange ExchangeObject { get; set; }
        public double Ask { get; set; }
        public double Bid { get; set; }

        public override string ToString()
        {
            return $"{ExchangeObject.name} {Ticker} - Ask: {Ask}, Bid: {Bid}";
        }
    }
}
