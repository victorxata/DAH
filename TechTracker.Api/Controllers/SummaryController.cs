using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using log4net;
using TechTracker.Api.Common.Attributes;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Services.Interfaces;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Validation]
    [RoutePrefix("api")]
    public class SummaryController : BaseController
    {
        private readonly ISummaryService _summaryService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="summaryService"></param>
        public SummaryController(ILog logger, ISummaryService summaryService) : base(logger)
        {
            _summaryService = summaryService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the summary
        /// </summary>
        /// <returns>The complete list of summary</returns>
        [HttpGet]
        [Route("summary")]
        public async Task<IHttpActionResult> GetSummary()
        {
            var result = await _summaryService.GetSummaryAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

       

    }
}
