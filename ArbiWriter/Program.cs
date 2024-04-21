using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiWriter.Services.Impl;
using ArbiWriter.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string conString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
builder.Services.AddDbContext<ArbiDbContext>(options =>
    options.UseMySql(conString, serverVersion));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<TokenCollectorService>();
builder.Services.AddScoped<IRepository<ExchangeToken, long>, TokenRepository>();
builder.Services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddScoped<ITokenCollector, TokenCollectorService>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
