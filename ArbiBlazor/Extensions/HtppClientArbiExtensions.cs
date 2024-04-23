using ArbiDataLib.Data;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ArbiBlazor.Extensions
{
    public static class HtppClientArbiExtensions
    {
        public static async Task<BasicResponse?> GetBasicAsync(this HttpClient http, string url)
        {
            try
            {
                return await http.GetFromJsonAsync<BasicResponse>(url);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static bool IsValidResponse(this BasicResponse? response)
        {
            if(response is null) return false;
            if (!response.Success) return false;

            return true;
        }

        public static T? TryParseContent<T>(this BasicResponse? response) where T : class
        {
            if (!response.IsValidResponse())
                return null;

            if (response!.Data is null)
                return null;

            return ((JsonElement)response.Data).Deserialize<T>();
        }
    }
}
