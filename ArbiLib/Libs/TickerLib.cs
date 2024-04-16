using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArbiLib.Libs
{
    internal class TickerLib
    {
        public static string GetPureTicker(string str)
        {
            string[] pair = str.Split("/");
            if (pair.Length == 2)
            {
                return pair[0];
            }
            return str;
        }

        public static bool IsUsdtPair(string str)
        {
            string[] pair = str.Split("/");
            if (pair.Length == 2)
            {
                return pair[1] == "USDT";
            }
            return false;
        }

        public static string RemoveSemiColon(string str)
        {
            int index = str.IndexOf(':');
            if (index != -1)
            {
                string result = str.Substring(0, index);
                return result;
            }

            return str;

        }

        public static double CalculatePercentDifference(double A, double B)
        {
            // Разница между ценами
            double difference = B - A;

            // Процентная разница
            double percentDifference = (difference / A) * 100;

            return percentDifference;
        }
    }
}
