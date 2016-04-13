using System.Web.Http;
using log4net;
using TechTracker.Api.Common.Attributes;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Validation]
    [Log]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public ILog Log;

        /// <summary>
        /// 
        /// </summary>
        public BaseController(ILog logger)
        {
            Log = logger;
        }
    }
}
