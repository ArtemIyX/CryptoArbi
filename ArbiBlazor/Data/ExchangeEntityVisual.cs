using ArbiDataLib.Models;
using Microsoft.AspNetCore.Components;

namespace ArbiBlazor.Data
{
    public class ExchangeEntityVisual
    {
        public ExchangeEntityResponse? Item { get; set; } = null;
        public bool Flag { get; set; } = true;

        public ExchangeEntityVisual()
        {
            Item = null;
            Flag = false;
        }

        public ExchangeEntityVisual(ExchangeEntityResponse item, bool flag)
        {
            Item = item;
            Flag = flag;
        }

        public void FlagChanged(ChangeEventArgs e)
        {
            // get the checkbox state
            object? value = e.Value;
            if(value is not null and bool)
            {
                Flag = (bool)value;
            }  
        }

    }
}
