using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TechTracker.Common.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Checks whether the string is NULL or empty.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the string is NULL or empty, otherwise false.</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Checks whether the string is not NULL or empty.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>True if the string is not NULL or empty, otherwise false.</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !value.IsNullOrEmpty();
        }

        /// <summary>
        /// Splits a delimited string into an enumerable collection.
        /// </summary>
        /// <param name="delimitedString">The delimited string to split.</param>
        /// <param name="separator">The delimiter to split on.</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitToEnumerable(this string delimitedString, params char[] separator)
        {
            IList<string> list = null;

            if (!delimitedString.IsNullOrEmpty())
                list = new List<string>(delimitedString.Split(separator));

            return list;
        }

        public static MemoryStream MakeMemoryStream(this string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value));
        }

        public static bool IsLowerEquals(string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

    }
}