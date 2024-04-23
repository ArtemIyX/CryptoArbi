using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiWriter.Services;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            IHost host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }
        catch(Exception ex)
        {
            await Console.Out.WriteLineAsync(ex.Message);
        }
       
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                IConfiguration configuration = context.Configuration;
                string conString = configuration.GetConnectionString("DefaultConnection") ?? "";

                MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

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