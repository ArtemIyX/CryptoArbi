using System.Globalization;

namespace ArbiBlazor.Services
{
    public interface IDisplaySerivce
    {
        public string Volume(double price);
        public string Percent(double percent);
        public string Price(double price);
        public string Fee(double? fee);
        public string Trade(string url, string symbolName);
        public string Depo(string url, string symbolName);
    }

    public class DisplayService : IDisplaySerivce
    {

        private string ConvertDoubleToString(double value)
        {
            // Define suffixes for thousands, millions, billions, etc.
            string[] suffixes = ["", "k", "kk", "kkk", "kkkk"]; // Add more as needed

            // Determine the appropriate suffix to use based on the magnitude of the value
            int suffixIndex = 0;
            while (Math.Abs(value) >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                value /= 1000;
                suffixIndex++;
            }

            // Format the value with the determined suffix
            string formattedValue = $"{value:F2}{suffixes[suffixIndex]}";
            return formattedValue;
        }

        public string Price(double price)
            => price.ToString("0.########################", CultureInfo.InvariantCulture);

        public string Percent(double percent)
            => "+" + percent.ToString("0.00", CultureInfo.InvariantCulture) + "%";

        public string Volume(double price)
            => ConvertDoubleToString(price);

        public string Trade(string str, string symbolName)
        {
            string lower = "btc";
            string upper = "BTC";

            if (str.Contains(lower))
            {
                return str.Replace(lower, symbolName.ToLower()).Replace("USDT", "usdt");
            }
            else if(str.Contains(upper))
            {
                return str.Replace(upper, symbolName.ToUpper()).Replace("usdt", "USDT");
            }
            return str;
        }

        public string Depo(string str, string symbolName)
        {
            string lower = "btc";
            string upper = "BTC";

            if (str.Contains(lower))
            {
                return str.Replace(lower, symbolName.ToLower());
            }
            else if (str.Contains(upper))
            {
                return str.Replace(upper, symbolName.ToUpper());
            }
            return str;
        }

        public string Fee(double? fee) => fee is null ? "???" : fee.Value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
