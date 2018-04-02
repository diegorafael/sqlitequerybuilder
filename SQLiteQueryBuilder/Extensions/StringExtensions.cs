using System;
using System.Linq;
using System.Reflection;

namespace SQLiteQueryBuilder.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string text)
        {
            bool ret = false;
            decimal dec;

            if (!string.IsNullOrWhiteSpace(text))
                if (decimal.TryParse(text, out dec))
                    ret = true;

            return ret;
        }
    }
}
