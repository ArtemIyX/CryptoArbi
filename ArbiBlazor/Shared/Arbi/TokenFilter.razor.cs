using ArbiBlazor.Data;
using ArbiDataLib.Data;
using ArbiDataLib.Models;
using Microsoft.AspNetCore.Components;

namespace ArbiBlazor.Shared.Arbi
{
    public partial class TokenFilter : ComponentBase
    {
        [Parameter]
        public IList<ExchangeEntityVisual> BuyItems { get; set; } = [];

        [Parameter]
        public IList<ExchangeEntityVisual> SellItems { get; set; } = [];

        [Parameter]
        public ArbiFilter Filter { get; set; } = new ArbiFilter();

        public static string MakeBuyId(string id) => $"{id}_buy";
        public static string MakeSellId(string id) => $"{id}_sell";

        public void OnAmountChange(ChangeEventArgs e)
        {
            if(e.Value is not null)
            {
                if (double.TryParse(e.Value as string, out double res))
                {
                    Filter.Amount = (int)res;
                }
            }
        }

        public void OnDayVolumeChange(ChangeEventArgs e) => _ = TryGetDoubleValue(e, out double res) ? Filter.MinDayVolumeUSDT = res : 0.0;
        public void OnMinPriceChange(ChangeEventArgs e) => _ = TryGetDoubleValue(e, out double res) ? Filter.MinPrice = res : 0.0;
        public void OnVolumeChange(ChangeEventArgs e) => _ = TryGetDoubleValue(e, out double res) ? Filter.MinVolumeUSDT = res : 0.0;
        public void OnMinPercent(ChangeEventArgs e) => _ = TryGetDoubleValue(e, out double res) ? Filter.MinPercent = res : 0.0;
        public void OnMaxPercent(ChangeEventArgs e) => _ = TryGetDoubleValue(e, out double res) ? Filter.MaxPercent = res : 0.0;

        private bool TryGetDoubleValue(ChangeEventArgs e, out double res)
        {
            if (e.Value is not null)
            {
                if (double.TryParse(e.Value as string, out double temp))
                {
                    res = temp;
                    return true;
                }
            }
            res = 0.0;
            return false;
        }
    }
}