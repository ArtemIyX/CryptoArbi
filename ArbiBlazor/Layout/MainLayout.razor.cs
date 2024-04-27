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
            WorkingStatus = UpdateWorkingStatus();
            ImagePath = UpdateImagePath();
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