using System;
using System.Globalization;

namespace TechTracker.Common.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// This is Sajan's unique take on what constitutes an ISO 8601 compliant string.
        /// You'll find that it matches the Xliff 1.2 recommendation.
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToISO8601String(this DateTime datetime)
        {
            //http://msdn.microsoft.com/en-us/library/az4se3k1.aspx#Roundtrip
            //We take the format string that "o" implies, and remove the .fffffff part... so that the
            //extra precision doesn't confuse folks who don't fully support ISO8601...
            return datetime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK", CultureInfo.InvariantCulture);
        }
    }
}