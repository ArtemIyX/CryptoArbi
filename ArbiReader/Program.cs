using ArbiDataLib.Data;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiReader.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string conString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
MySqlServerVersion serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
builder.Services.AddDbContext<ArbiDbContext>(options =>
    options.UseMySql(conString, serverVersion));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepository<ExchangeToken, long>, TokenRepository>();
builder.Services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
