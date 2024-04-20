using ArbiWriter.Data;
using ArbiWriter.Data.Repo;
using ArbiWriter.Models;
using ArbiWriter.Services.Impl;
using ArbiWriter.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string conString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
builder.Services.AddDbContext<ArbiDbContext>(options =>
    options.UseMySQL(conString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<TokenCollectorService>();
builder.Services.AddScoped<IRepository<ExchangeToken, int>, TokenRepository>();
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
