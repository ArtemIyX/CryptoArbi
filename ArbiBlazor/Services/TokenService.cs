using ArbiBlazor.Extensions;
using ArbiDataLib.Data;
using ArbiDataLib.Models;

namespace ArbiBlazor.Services
{
    public interface ITokenService
    {
        public Task<List<ArbiItem>> GetArbiItems(ArbiFilter filter);
    }
    public class TokenService(HttpClient http) : ITokenService
    {
        private readonly HttpClient _http = http;
        private readonly string ArbiUrl = "/api/tokens/arbi";

        public async Task<List<ArbiItem>> GetArbiItems(ArbiFilter filter)
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync(ArbiUrl);
                List<ArbiItem>? list = response
                    .TryParseContent<List<ArbiItem>>();
                return list ?? [];
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
