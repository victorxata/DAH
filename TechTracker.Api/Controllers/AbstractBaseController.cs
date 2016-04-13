using System;
using System.Web;
using System.Web.Http;
using log4net;
using Microsoft.AspNet.Identity;
using TechTracker.Api.Common.Attributes;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// Base class for functionality common to all TX Api Controllers
    /// </summary>
    [Authorize]
    public abstract class AbstractBaseController : BaseController
    {
      
        /// <summary>
        /// The id of the tenant that was sent to us in the request headers or null if no Tenant was specified.
        /// </summary>
        protected string TenantId => HttpContext.Current != null ? HttpContext.Current.Request.Headers["TenantId"] : string.Empty;


        /// <summary>
        /// User name in the current request. We are getting the Identity value. If there is no user in the Identity, throw an error
        /// </summary>
        protected string UserName
        {
            get
            {
                try
                {
                    return User.Identity.GetUserName();
                    
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        protected AbstractBaseController(ILog logger) : base(logger)
        {
        }
    }
}