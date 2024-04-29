
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
            tasks.Add(UpdateTokens());
            await Task.WhenAll(tasks);
        }

        public async Task UpdateTokens()
        {
            ArbiFilter filter = filterContainer.CurrentFilter;
            var buys = filterContainer.BuyExchanges;
            arbiContainer.Items = null;
            IList<ArbiItemVisual> items = await arbiService.GetArbiItems(filter);
            arbiContainer.Items = items;
        }

    }
}
