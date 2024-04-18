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

        public double CalculateProfit(ArbiOportunity ArbiOportunity,
            double BalanceUsdt, 
            double TakerFeeAsk,
            double TakerFeeBid,
            double WithdrawFeeAmount,
            double UsdtWithdrawFeeAmount)
        {
            if (!ArbiOportunity.IsValid)
                return 0.0;

            Arbi askArbi = ArbiOportunity.MinimalAsk!;
            Arbi bidArbi = ArbiOportunity.MaximalBid!;

            double ask = askArbi.Ask;
            double bid = bidArbi.Bid;

            double amountToBuy = BalanceUsdt / ask;

            double amountReceived = amountToBuy - (amountToBuy * TakerFeeAsk);

            double amountTransferred = amountReceived - WithdrawFeeAmount;

            double amountSold = amountTransferred * bid;

            double amountUsdtReceived = amountSold - (amountSold * TakerFeeBid);

            double finalBalance = (amountUsdtReceived - UsdtWithdrawFeeAmount);

            return finalBalance - BalanceUsdt;
        }
    }
}
