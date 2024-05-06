using ArbiDataLib.Models;
using Microsoft.AspNetCore.Components;

namespace ArbiBlazor.Pages
{
    public partial class Arbi : ComponentBase
    {
        [Parameter]
        public string? Sym { get; set; }

        public IList<ExchangeTokenVisual>? Total { get; set; } = null;
        public IList<ExchangeTokenVisual>? Ask { get; set; } = null;
        public IList<ExchangeTokenVisual>? Bid { get; set; } = null;

        protected override async Task OnInitializedAsync()
        {
            await Refresh();
        }
        
        private async Task Back()
        {
            navManager.NavigateTo("/tokens");
        }
        
        private async Task Refresh()
        {
            if (string.IsNullOrEmpty(Sym))
            {
                Ask = null;
                Bid = null;
                Total = null;
                return;
            }
            IList<ExchangeTokenVisual> res = await arbiService.GetArbiSituation(Sym);
            Ask = [.. res.OrderBy(x => x.Ask)];
            Bid = [.. res.OrderByDescending(x => x.Bid)];
            Total = res;
        }
    }
}