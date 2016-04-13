using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TechTracker.Api.Common.Handlers;

namespace TechTracker.Api
{
    /// <summary>
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );

            config.Filters.Add(new ExceptionHandlingAttribute());

            EnableCamelCaseJson(config);
        }

        /// <summary>
        ///     Enables indented camelCase for ease of reading.
        /// </summary>
        /// <param name="config">HttpConfiguration for this api</param>
        private static void EnableCamelCaseJson(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var stringEnumConverter = new StringEnumConverter {CamelCaseText = true};
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(stringEnumConverter);
            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}