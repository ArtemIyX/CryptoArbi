using ArbiDataLib.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArbiBlazor.Pages
{

    public partial class ArbiTokens : ComponentBase
    {
        public List<ArbiItem> Tokens { get; set; } = [];

        protected override async Task OnInitializedAsync()
        {
            await UpdateTokens();
        }

        public async Task UpdateTokens()
        {
            Tokens.Clear();
            Tokens = await tokenService.GetArbiItems(new ArbiFilter());
        }
    }
}
