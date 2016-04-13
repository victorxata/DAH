using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using log4net;
using TechTracker.Common.Tracer;
using TechTracker.Common.Utils.Configuration;

namespace TechTracker.Api.Common.Handlers
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public static ILog Log = new LoggerWrapper(new AppConfigManager());

        
        public override void OnException(HttpActionExecutedContext context)
        {
            
            Log.Fatal(context.Exception.Message, context.Exception);

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(context.Exception.Message),
                ReasonPhrase = "Exception"
            });
        }
    }
}
