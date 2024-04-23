using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiWriter.Services.Impl;
using ArbiWriter.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Получаем строку подключения из конфигурации
                IConfiguration configuration = context.Configuration;
                string conString = configuration.GetConnectionString("DefaultConnection") ?? "";

                MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

                // Регистрируем сервисы в контейнере зависимостей
                services.AddDbContext<ArbiDbContext>(options =>
                    options.UseMySql(conString, serverVersion));

                services.AddHostedService<TokenCollectorService>();
                services.AddScoped<IRepository<ExchangeToken, long>, TokenRepository>();
                services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
                services.AddScoped<ITokenService, TokenService>();
                services.AddScoped<IExchangeService, ExchangeService>();
                services.AddScoped<ITokenCollector, TokenCollectorService>();
            });
}