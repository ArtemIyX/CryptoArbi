using ArbiBlazor.Extensions;
using ArbiDataLib.Data;
using ArbiDataLib.Models;

namespace ArbiBlazor.Services
{
    public interface ITokenService
    {
        
    }
    public class TokenService(HttpClient http) : ITokenService
    {
        private readonly HttpClient _http = http;

    }
}
