namespace ArbiWriter.Services.Interfaces
{
    public interface ITokenCollector
    {
        public Task PrepareExchanges(CancellationToken stoppingToken = default);
        public Task TurnOff(CancellationToken stoppingToken = default);
    }
}
