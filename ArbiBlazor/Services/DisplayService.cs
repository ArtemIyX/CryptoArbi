using ccxt;
using System.Globalization;

namespace ArbiBlazor.Services
{
    public interface IDisplaySerivce
    {
        public string Volume(double price);
        public string Percent(double percent);
        public string Price(double price);
    }
    public class DisplayService : IDisplaySerivce
    {
        public string Price(double price)
            => price.ToString(CultureInfo.InvariantCulture);

        public string Percent(double percent)
            => percent.ToString("0.00", CultureInfo.InvariantCulture) + "%";

        public string Volume(double price)
            => price.ToString("0.00", CultureInfo.InvariantCulture) + "$";
    }
}
