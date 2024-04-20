using ArbiDataLib.Data;
using ArbiDataLib.Data.Data.Repo;
using ArbiDataLib.Data.Repo;
using ArbiDataLib.Models;
using ArbiReader.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string conString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
builder.Services.AddDbContext<ArbiDbContext>(options =>
    options.UseMySQL(conString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepository<ExchangeToken, int>, TokenRepository>();
builder.Services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
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
