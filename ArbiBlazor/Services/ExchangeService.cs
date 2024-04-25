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
        public Task<IList<ExchangeEntityResponse>> GetExchanges();
        public Task<IList<ExchangeEntityResponse>> GetWorkingExchanges();
        public Task<ExchangeEntityResponse?> GetExchange(string exchangeId);
        public Task<ExchangeUrlInfo?> GetExchangeUrlInfo(string exchangeId);
        public Task<IList<ExchangeUrlInfo>> GetExchangeUrlInfos();
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

        public async Task<IList<ExchangeUrlInfo>> GetExchangeUrlInfos()
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync(ExchangesInfoUrl);
                return response.TryParseContent<List<ExchangeUrlInfo>>() ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }


        public async Task<IList<ExchangeEntityResponse>> GetExchanges()
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


        public async Task<IList<ExchangeEntityResponse>> GetWorkingExchanges()
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
