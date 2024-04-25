using ArbiDataLib.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArbiBlazor.Pages
{

    public partial class ArbiTokens : ComponentBase
    {
        private readonly string CardDefaultStyle = "bg-light";
        private readonly string CardHoveredStyle = "bg-primary";
        public string CardCssStyle { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            CardCssStyle = CardDefaultStyle;
            if (arbiContainer.Items is null ||
                arbiContainer.Items?.Count == 0)
            {
                await UpdateTokens();
            }
        }

        public async Task UpdateTokens()
        {
            var res = await arbiService.GetArbiItems(new ArbiFilter());
            arbiContainer.Items = res;
        }
    }
}
