using ArbiDataLib.Models;

namespace ArbiReader.Data.Responses
{
    public class ExchangeListResponse
    {
        public ExchangeListResponse()
        {

        }

        public ExchangeListResponse(IList<ExchangeEntityResponse> items)
        {
            Items = items;
        }

        public IList<ExchangeEntityResponse> Items { get; set; } = [];
    }
}
