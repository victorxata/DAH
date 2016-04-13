using System;
using System.Text;

namespace TechTracker.Common.Utils.Extensions
{
    public static class StringBuilderExtensions
    {
        public static bool StartsWith(this StringBuilder sb, string value, StringComparison comparisonType)
        {
            if (sb.Length < value.Length)
                return false;

            var start = sb.ToString(0, value.Length);
            return start.Equals(value, comparisonType);
        }

        public static bool StartsWith(this StringBuilder sb, string value)
        {
            return StartsWith(sb, value, StringComparison.CurrentCulture);
        }

        public static bool EndsWith(this StringBuilder sb, string value, StringComparison comparisonType)
        {
            if (sb.Length < value.Length)
                return false;

            var end = sb.ToString(sb.Length - value.Length, value.Length);
            return end.Equals(value, comparisonType);
        }

        public static bool EndsWith(this StringBuilder sb, string value)
        {
            return EndsWith(sb, value, StringComparison.CurrentCulture);
        }
    }
}