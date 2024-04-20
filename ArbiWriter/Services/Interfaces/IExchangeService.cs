using ArbiDataLib.Models;
using ccxt;

namespace ArbiWriter.Services.Interfaces
{
    public interface IExchangeService
    {
        public ICollection<ccxt.Exchange> GetSupportedExchanges();
        public Task MarkAllAsWorkingAsync(bool bWorking = true, CancellationToken stoppingToken = default);
        public ExchangeEntity CreateExchangeEntity(Exchange exchange);
        public Task UploadData(CancellationToken stoppingToken = default);
    }
}
