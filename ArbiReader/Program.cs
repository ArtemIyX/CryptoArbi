using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiReader.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();


    builder.Configuration.AddJsonFile(path:"exchanges.json", optional:true, reloadOnChange:true);

    string conString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
    MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
    builder.Services.AddDbContext<ArbiDbContext>(options =>
        options.UseMySql(conString, serverVersion));

    builder.WebHost.UseKestrel(x =>
    {
        x.Listen(new IPEndPoint(
            IPAddress.Parse(builder.Configuration["ServerIp"]),
            int.Parse(builder.Configuration["ServerPort"]
            )), configure =>
            {
                configure.UseHttps();
            });
    });
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddScoped<IRepository<ExchangeToken, long>, TokenRepository>();
    builder.Services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
    builder.Services.AddScoped<IExchangeService, ExchangeService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IExchangeInfoService, ExchangeInfoService>();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseCors();
    app.UseHttpsRedirection();

    app.UseAuthorization();
    
    app.MapControllers();
   // app.UseSerilogRequestLogging();

    await app.RunAsync();
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    
}