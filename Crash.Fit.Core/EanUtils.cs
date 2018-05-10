using System;
using System.Collections.Generic;
using System.Text;

namespace Crash.Fit
{
    public class EanUtils
    {
        public static bool IsEan13(string ean)
        {
            if(string.IsNullOrWhiteSpace(ean) || ean.Length != 13)
            {
                return false;
            }

            return IsAllNumbers(ean);
        }
        public static bool IsInternalEan13(string ean)
        {
            if(!IsEan13(ean))
            {
                return false;
            }
            var prefix = int.Parse(ean.Substring(0, 3));
            return prefix >= 200 && prefix <= 299;
        }
        public static string NormalizeInternalEan13(string ean)
        {
            ean = ean.Substring(0, 8) + "0000";
            ean += CalculateCheckNumberEan13(ean);
            return ean;
        }
        public static bool IsAllNumbers(string text)
        {
            foreach (var c in text)
            {
                if (!char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }
        private static int CalculateCheckNumberEan13(string ean)
        {
            var sum = 0;
            for(var i = 0; i <= 11; i++)
            {
                var number = int.Parse(ean.Substring(i, 1));
                if(i % 2 == 0)
                {
                    sum += number;
                }
                else
                {
                    sum += 3 * number;
                }
            }
            var mod = sum % 10;
            return mod == 0 ? 0 : 10 - mod;
        }
    }
}
