using ArbiDataLib.Data;
using ArbiDataLib.Models;

namespace ArbiBlazor.Services
{
    public interface IFilterContainer
    {
        ArbiFilter CurrentFilter { get; set; }
        IList<ExchangeEntityResponse>? Exchanges { get; set; }
        IList<ExchangeEntityVisual> BuyExchanges { get; set; }
        IList<ExchangeEntityVisual> SellExchanges { get; set; }
    }

    public class FilterContainer : IFilterContainer
    {
        private ArbiFilter _currentFilter = new();
        public ArbiFilter CurrentFilter { get => _currentFilter; set => _currentFilter = value; }

        private IList<ExchangeEntityResponse>? _exchanges = [];
        public IList<ExchangeEntityResponse>? Exchanges { get => _exchanges; set => _exchanges = value; }

        private IList<ExchangeEntityVisual> _buyExchanges = [];
        public IList<ExchangeEntityVisual> BuyExchanges { get => _buyExchanges; set => _buyExchanges = value; }

        private IList<ExchangeEntityVisual> _sellExchanges = [];
        public IList<ExchangeEntityVisual> SellExchanges { get => _sellExchanges; set => _sellExchanges = value; }
    }
}
