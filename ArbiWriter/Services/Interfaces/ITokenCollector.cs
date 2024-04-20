namespace ArbiWriter.Services.Interfaces
{
    public interface ITokenCollector
    {
        public Task PrepareExchanges(CancellationToken stoppingToken = default);
    }
}
