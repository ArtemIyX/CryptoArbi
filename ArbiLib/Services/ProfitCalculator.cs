using ArbiLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiLib.Services
{
    public class ProfitCalculator(ArbiService InService)
    {
        public ArbiService ArbiService { get; private set; } = InService;

        double CalculateProfit(double BalanceUsdt, ArbiOportunity ArbiOportunity)
        {
            if (!ArbiOportunity.IsValid)
                return 0.0;

            Arbi askArbi = ArbiOportunity.MinimalAsk!;
            Arbi bidArbi = ArbiOportunity.MaximalBid!;

            double ask = askArbi.Ask;
            double bid = bidArbi.Bid;


            return 0.0;
        }
    }
}
