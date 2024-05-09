using ArbiDataLib.Data;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace ArbiBlazor.Services
{
    public interface IArbiContainer
    {
        public IList<ArbiItemVisual>? Items { get; set; }
        public int ProfitUsdt { get; set; }
        public void ProfitChanged(ChangeEventArgs e);
    }

    public class ArbiContainer : IArbiContainer
    {
        private IList<ArbiItemVisual>? _items = null;
        public IList<ArbiItemVisual>? Items
        {
            get => _items;
            set => _items = value;
        }

        private int _profitUsdt = 500;
        public int ProfitUsdt
        {
            get => _profitUsdt;
            set => _profitUsdt = value;
        }

        public void ProfitChanged(ChangeEventArgs e)
        {
            object? value = e.Value;
            if (value is not null and string)
            {
                if (int.TryParse((string)value, out int res))
                {
                    ProfitUsdt = res;
                }
            }
        }
    }
}
