using ArbiBlazor.Extensions;
using ArbiDataLib.Data;
using ArbiDataLib.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;

namespace ArbiBlazor.Services
{
    public interface IExchangeService
    {
        public Task<List<ExchangeEntityResponse>> GetExchanges();
        public Task<List<ExchangeEntityResponse>> GetWorkingExchanges();
        public Task<ExchangeEntityResponse?> GetExchange(string exchangeId);
        public Task<ExchangeUrlInfo?> GetExchangeUrlInfo(string exchangeId);
    }

    public class ExchangeService(HttpClient httpClient) : IExchangeService
    {
        private readonly HttpClient _http = httpClient;
        private readonly string WorkingExchangesUrl = "api/exchanges/working";
        private readonly string ExchangesUrl = "api/exchanges/";
        private readonly string ExchangesInfoUrl = "api/exchanges/u/";

        public async Task<ExchangeEntityResponse?> GetExchange(string exchangeId)
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync($"{ExchangesUrl}{exchangeId}");
                return response.TryParseContent<ExchangeEntityResponse>();
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<List<ExchangeEntityResponse>> GetExchanges()
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync(ExchangesUrl);
                List<ExchangeEntityResponse>? list = response
                    .TryParseContent<List<ExchangeEntityResponse>>();
                return list ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<ExchangeUrlInfo?> GetExchangeUrlInfo(string exchangeId)
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync($"{ExchangesInfoUrl}{exchangeId}") ;
                ExchangeUrlInfo? content = response.TryParseContent<ExchangeUrlInfo>();
                return content;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ExchangeEntityResponse>> GetWorkingExchanges()
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync(WorkingExchangesUrl);
                List<ExchangeEntityResponse>? list = response
                    .TryParseContent<List<ExchangeEntityResponse>>();
                return list ?? [];
            }
            catch(Exception)
            {
                return [];
            }
        }
    }
}
