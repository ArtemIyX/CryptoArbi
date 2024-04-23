
using System.Text.Json.Serialization;

namespace ArbiDataLib.Data
{
    public class BasicResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; } = true;

        [JsonPropertyName("data")]
        public object? Data { get; set; } = null;

        [JsonPropertyName("message")]
        public string Message { get; set; } = "OK";

        [JsonPropertyName("code")]
        public int Code { get; set; } = 200;
    }
}
