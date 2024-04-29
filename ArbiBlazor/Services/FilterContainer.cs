using ArbiBlazor.Data;
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

        public string MakeForbiddenString(IList<ExchangeEntityVisual> entities);
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

        public string MakeForbiddenString(IList<ExchangeEntityVisual> entities)
        {
            IEnumerable<ExchangeEntityVisual> banned = entities.Where(x => !x.Flag);
            if (banned.Any())
            {
                return string.Join(',', banned.Select(x => x.Item?.Id));
            }
            return string.Empty;
        }
    }
}
