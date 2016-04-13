using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using log4net;
using TechTracker.Common.Tracer;
using TechTracker.Common.Utils.Configuration;

namespace TechTracker.Api.Common.Attributes
{
    public class LogAttribute: ActionFilterAttribute
    {
        public ILog Log;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Log = new LoggerWrapper(new AppConfigManager());

            var txt = "Request to "
                + actionContext.Request.RequestUri.OriginalString;

            Log.Debug(txt);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response == null) return;

            Log = new LoggerWrapper(new AppConfigManager());

            var txt = "Response ["
                      + actionExecutedContext.Response.StatusCode
                      + "] to "
                      + actionExecutedContext.ActionContext.Request.RequestUri.OriginalString;

            Log.Debug(txt);
        }
    }
}
