
using ArbiBlazor.Data;
using ArbiBlazor.Services;
using ArbiDataLib.Models;
using Microsoft.AspNetCore.Components;

namespace ArbiBlazor.Layout
{
    public partial class MainLayout
    {
        public bool? IsWorking { get; set; } = null;
        public string WorkingStatus { get; set; } = "";
        public string ImagePath { get; set; } = "svg/loading.svg";

        protected override async Task OnInitializedAsync()
        {
            WorkingStatus = UpdateWorkingStatus();
            IsWorking = await appStatusService.Ping();
            if (filterContainer.Exchanges is null ||
    filterContainer.Exchanges?.Count == 0)
            {
                await UpdateExchangs();
            }
           
            WorkingStatus = UpdateWorkingStatus();
            ImagePath = UpdateImagePath();
        }
        public async Task UpdateExchangs()
        {
            IList<ExchangeEntityResponse> res = await exchangeService.GetWorkingExchanges();
            filterContainer.Exchanges = res;
            filterContainer.BuyExchanges = filterContainer.Exchanges.Select(x => new ExchangeEntityVisual(x, true)).ToList();
            filterContainer.SellExchanges = filterContainer.Exchanges.Select(x => new ExchangeEntityVisual(x, true)).ToList();
        }


        protected string UpdateImagePath()
        {
            if (IsWorking == null)
            {
                return "svg/loading.svg";
            }
            else if (IsWorking == true)
            {
                return "svg/ok.svg";
            }
            else
            {
                return "svg/no.svg";
            }
        }
        protected string UpdateWorkingStatus()
        {
            if (IsWorking == null)
            {
                return "Loading";
            }
            else if (IsWorking == true)
            {
                return "Online";
            }
            else
            {
                return "Offline";
            }
        }
    }
}