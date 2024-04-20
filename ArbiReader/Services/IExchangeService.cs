using ArbiDataLib.Models;

namespace ArbiReader.Services
{
    public interface IExchangeService
    {
        public Task<IList<ExchangeEntityResponse>> Get();
        public Task<IList<ExchangeEntityResponse>> Working();
    }
}
