using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiReader.Services;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.X509Certificates;

/*try
{*/
    var builder = WebApplication.CreateBuilder(args);


    // Add services to the container.
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


    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<IRepository<ExchangeToken, long>, TokenRepository>();
    builder.Services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
    builder.Services.AddScoped<IExchangeService, ExchangeService>();
    builder.Services.AddScoped<ITokenService, TokenService>();

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

    app.Run();
/*}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
*/