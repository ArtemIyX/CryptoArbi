namespace ArbiBlazor.Services
{
    public interface IAppSettingsService
    {
        public string ServerUrl { get; }
    }

    public class AppSettingsService(IConfiguration configuration) : IAppSettingsService
    {
        private readonly IConfiguration _config = configuration;
        public string ServerUrl => _config.GetValue<string>("Api") ?? "";
    }
}
