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

        protected override async Task OnInitializedAsync()
        {

        }

        public static string MakeBuyId(string id) => $"{id}_buy";
        public static string MakeSellId(string id) => $"{id}_sell";
    }
}