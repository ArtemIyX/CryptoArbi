﻿using System.Globalization;

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
    }
}