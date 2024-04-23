using ArbiBlazor.Services;
using ArbiDataLib.Models;

namespace ArbiBlazor.Pages
{
    public partial class Exchanges
    {
        public List<ExchangeEntityResponse> ExchangeEntities { get; set; } = [];
        protected override async Task OnInitializedAsync()
        {
            ExchangeEntities = await exchangeService.GetExchanges();
        }
    }
}