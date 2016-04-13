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
    public class CandidatesController : BaseController
    {
        private readonly ICandidatesService _candidatesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="candidatesService"></param>
        public CandidatesController(ILog logger, ICandidatesService candidatesService) : base(logger)
        {
            _candidatesService = candidatesService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the candidates with all the properties that can be securized
        /// </summary>
        /// <returns>The complete list of candidates</returns>
        [HttpGet]
        [Route("candidates")]
        public async Task<IHttpActionResult> GetCandidates()
        {
            var result = await _candidatesService.GetCandidateAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve a candidate
        /// </summary>
        /// <param name="candidateId">The Id of the candidate</param>
        /// <returns>The candidate that matches the given Id</returns>
        [HttpGet]
        [Route("candidates/{candidateId}")]
        public async Task<IHttpActionResult> GetCandidateById(string candidateId)
        {
            var result = await _candidatesService.GetCandidateByIdAsync(candidateId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new candidate. 
        /// </summary>
        /// <param name="candidate">The candidate object to add</param>
        /// <returns>201 Created response and a header with the location of the new candidate created</returns>
        [HttpPost]
        [Route("candidates")]
        public async Task<IHttpActionResult> PostCandidate([FromBody] Candidate candidate)
        {
            var permissionTypeCreated = await _candidatesService.AddCandidateAsync(candidate);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionTypeCreated.Id}";
            var response = Created(create, permissionTypeCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing candidate.
        /// </summary>
        /// <param name="candidateId">The Id of the candidate to retrieve</param>
        /// <param name="candidate">The object of the new candidate body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpPut]
        [Route("candidates/{candidateId}")]
        public async Task<IHttpActionResult> UpdateCandidate(string candidateId, [FromBody] Candidate candidate)
        {
            if (candidate.Id != candidateId)
                return BadRequest("Field Permission Id does not match with request id");
            var result = await _candidatesService.UpdateCandidateAsync(candidate);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission. 
        /// </summary>
        /// <param name="candidateId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpDelete]
        [Route("candidates/{candidateId}")]
        public async Task<IHttpActionResult> DeleteCandidate(string candidateId)
        {
            await _candidatesService.DeleteCandidateAsync(candidateId);
            return Ok();
        }

    }
}
