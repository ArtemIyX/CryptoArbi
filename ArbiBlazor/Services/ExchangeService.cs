using ArbiDataLib.Data;
using ArbiDataLib.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;

namespace ArbiBlazor.Services
{
    public interface IExchangeService
    {
        public Task<List<ExchangeEntityResponse>> GetWorkingExchanges();
        public Task<ExchangeEntityResponse?> GetExchange(string exchangeId);
    }

    public class ExchangeService(HttpClient httpClient,
        IAppSettingsService appSettingsService) : IExchangeService
    {
        private readonly HttpClient _http = httpClient;
        private readonly IAppSettingsService _appSettings = appSettingsService;
        private readonly string WorkingExchangesUrl = "api/exchanges/working";

        public async Task<ExchangeEntityResponse?> GetExchange(string exchangeId)
        {
            return null;
        }

        public async Task<List<ExchangeEntityResponse>> GetWorkingExchanges()
        {
            BasicResponse? response = await _http.GetFromJsonAsync<BasicResponse>(WorkingExchangesUrl);
            if (response is null)
                return [];
            if (!response.Success)
                return [];
            if (response.Data is null)
                return [];
            List<ExchangeEntityResponse>? list = ((JsonElement)response.Data!)
                .Deserialize<List<ExchangeEntityResponse>>();
            if (list is null)
                return [];

            return list;
            try
            {
               
            }
            catch(Exception ex)
            {
                return [];
            }
        }
    }
}
