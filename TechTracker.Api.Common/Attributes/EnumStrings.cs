using System;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using Newtonsoft.Json.Converters;

namespace TechTracker.Api.Common.Attributes
{
    public class EnumStrings : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Formatters.Add(CreateSettings());
        }

        private static JsonMediaTypeFormatter CreateSettings()
        {
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            return formatter;
        }
    }
}
