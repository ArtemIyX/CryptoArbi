using ArbiDataLib.Data;
using ArbiDataLib.Models;

namespace ArbiBlazor.Services
{
    public interface IArbiContainer
    {
        public IList<ArbiItemVisual>? Items { get; set; }
    }

    public class ArbiContainer : IArbiContainer
    {
        private IList<ArbiItemVisual>? _items = null;
        public IList<ArbiItemVisual>? Items
        {
            get => _items;
            set => _items = value;
        }
    }
}
