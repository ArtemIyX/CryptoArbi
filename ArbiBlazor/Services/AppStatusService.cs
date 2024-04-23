using ArbiBlazor.Extensions;
using ArbiDataLib.Data;

namespace ArbiBlazor.Services
{
    public interface IAppStatusService
    {
        public Task<bool> Ping();
    }

    public class AppStatusService(HttpClient http) : IAppStatusService
    {
        private readonly HttpClient _http = http;

        public async Task<bool> Ping()
        {
            try
            {
                BasicResponse? response = await _http.GetBasicAsync("/api");
                if (response is null) return false;
                return response.Success;
            }
            catch(Exception)
            {
                return false;
            }
        }
    }
}
