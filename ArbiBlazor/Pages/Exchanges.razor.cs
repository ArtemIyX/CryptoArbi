using ArbiBlazor.Services;
using ArbiDataLib.Models;
using Microsoft.AspNetCore.Components;

namespace ArbiBlazor.Pages
{
    public partial class Exchanges : ComponentBase
    {
        public IList<ExchangeEntityResponse> ExchangeEntities { get; set; } = [];
        protected override async Task OnInitializedAsync()
        {
            ExchangeEntities = await exchangeService.GetExchanges();
        }
    }
}