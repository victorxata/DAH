using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechTracker.Common.Utils.Text
{
    public static class DataSourceHelpers
    {
        private static readonly Regex DataSourceRegex = new Regex("[D|d]ata [S|s]ource=(.+?);");

        /// <summary>
        /// Replace the Data Source attribute in from one connection string in another
        /// </summary>
        /// <param name="referenceConnectionString">The connection string with the Data Source you want to keep.</param>
        /// <param name="targetConnectionString">The connection string of a different format that needs a new Data Source.</param>
        /// <returns></returns>
        public static string SwitchDataSources(string referenceConnectionString, string targetConnectionString)
        {
            var referenceDataSource = DataSourceRegex.Match(referenceConnectionString).Value;

            var replacementConnectionString = DataSourceRegex.Replace(targetConnectionString, referenceDataSource);

            return replacementConnectionString;
        }
    }
}
