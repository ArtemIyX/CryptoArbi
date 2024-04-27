
using ArbiDataLib.Data;
using ArbiDataLib.Models;
using Microsoft.AspNetCore.Components;

namespace ArbiBlazor.Pages
{
    public partial class ArbiTokens : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await UpdateIfEmpty();
        }

        public async Task UpdateIfEmpty()
        {
            List<Task> tasks = [];

            if (filterContainer.Exchanges is null ||
                filterContainer.Exchanges?.Count == 0)
            {
                tasks.Add(UpdateExchangs());
            }
            if (arbiContainer.Items is null ||
                arbiContainer.Items?.Count == 0)
            {
                tasks.Add(UpdateTokens());
            }
            await Task.WhenAll(tasks);
        }

        public async Task RefreshAll()
        {
            List<Task> tasks = [];
            tasks.Add(UpdateExchangs());
            tasks.Add(UpdateTokens());
            await Task.WhenAll(tasks);
        }

        public async Task UpdateExchangs()
        {
            IList<ExchangeEntityResponse> res = await exchangeService.GetWorkingExchanges();
            filterContainer.Exchanges = res;
            filterContainer.BuyExchanges = filterContainer.Exchanges.Select(x => new ExchangeEntityVisual(x, true)).ToList();
            filterContainer.SellExchanges = filterContainer.Exchanges.Select(x => new ExchangeEntityVisual(x, true)).ToList();
        }

        public async Task UpdateTokens()
        {
            ArbiFilter filter = filterContainer.CurrentFilter;
            IList<ArbiItemVisual> items = await arbiService.GetArbiItems(filter);
            arbiContainer.Items = items;
        }

    }
}
