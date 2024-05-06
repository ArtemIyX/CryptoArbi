
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

        public async Task OnArbiClicked(ArbiItemVisual? arbiItem)
        {
            if (arbiItem is null)
                return;
            navManager.NavigateTo($"/tokens/{arbiItem.DisplayName}");
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
            arbiContainer.Items = null;
            IList<ArbiItemVisual> items = await arbiService.GetArbiItems(filter);
            arbiContainer.Items = items;
        }

    }
}
