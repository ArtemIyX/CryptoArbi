using ArbiDataLib.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiDataLib.Data.Repo
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepos(this IServiceCollection services)
        {
            services.AddScoped<IRepository<ExchangeTokenNetwork, long>, TokenNetworkRepository>();
            services.AddScoped<IRepository<ExchangeToken, long>, TokenRepository>();
            services.AddScoped<IRepository<ExchangeEntity, string>, ExchangeRepository>();
            services.AddScoped<IRepository<OrderBookItem, long>, OrderBookRepository>();
            return services;
        }
    }
}
