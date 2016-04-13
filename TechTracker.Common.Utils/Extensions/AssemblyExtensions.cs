using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TechTracker.Common.Utils.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the contents of a text file with the build action of Embedded Resource
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns></returns>
        public static string GetTextFromEmbeddedResource(this Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new ApplicationException(string.Format("Cannot find an embedded resource named {0}, available options are {1}", resourceName,
                        assembly.GetManifestResourceNames()));
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Gets JTokens from an embedded JSON array resource.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName">Thye name of the resource.</param>
        /// <returns></returns>
        public static IEnumerable<JToken> GetJTokensFromEmbeddedJsonArrayResource(this Assembly assembly, string resourceName)
        {
            var json = assembly.GetTextFromEmbeddedResource(resourceName);

            var rayaway = (JArray) JsonConvert.DeserializeObject(json);
            return rayaway;
        }


        /// <summary>
        /// Gets JTokens from an embedded JSON object resource.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourceName">Thye name of the resource.</param>
        /// <returns></returns>
        public static JObject GetJTokensFromEmbeddedJsonObjectResource(this Assembly assembly, string resourceName)
        {
            var json = assembly.GetTextFromEmbeddedResource(resourceName);

            var rayaway = (JObject) JsonConvert.DeserializeObject(json);
            return rayaway;
        }
    }
}