
namespace ArbiReader.Data
{
    public class BasicResponse
    {
        public bool Success { get; set; } = true;
        public object? Data { get; set; } = null;
        public required string Message { get; set; } = "OK";
        public int Code { get; set; } = 200;
    }
}
