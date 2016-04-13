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
    public class OpportunitiesController : BaseController
    {
        private readonly IOpportunitiesService _opportunitiesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="opportunitiesService"></param>
        public OpportunitiesController(ILog logger, IOpportunitiesService opportunitiesService) : base(logger)
        {
            _opportunitiesService = opportunitiesService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the opportunities with all the properties that can be securized
        /// </summary>
        /// <returns>The complete list of opportunities</returns>
        [HttpGet]
        [Route("opportunities")]
        public async Task<IHttpActionResult> GetOpportunities()
        {
            var result = await _opportunitiesService.GetOpportunityAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve an opportunity
        /// </summary>
        /// <param name="oppId">The Id of the opportunity</param>
        /// <returns>The opportunity that matches the given Id</returns>
        [HttpGet]
        [Route("opportunities/{oppId}")]
        public async Task<IHttpActionResult> GetOpportunityById(string oppId)
        {
            var result = await _opportunitiesService.GetOpportunityByIdAsync(oppId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new opportunity. 
        /// </summary>
        /// <param name="opp">The opportunity object to add</param>
        /// <returns>201 Created response and a header with the location of the new opportunity created</returns>
        [HttpPost]
        [Route("opportunities")]
        public async Task<IHttpActionResult> PostOpportunity([FromBody] Opportunity opp)
        {
            var permissionTypeCreated = await _opportunitiesService.AddOpportunityAsync(opp);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionTypeCreated.Id}";
            var response = Created(create, permissionTypeCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing opportunity.
        /// </summary>
        /// <param name="oppId">The Id of the opportunity to retrieve</param>
        /// <param name="opp">The object of the new opportunity body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpPut]
        [Route("opportunities/{oppId}")]
        public async Task<IHttpActionResult> UpdateOpportunity(string oppId, [FromBody] Opportunity opp)
        {
            if (opp.Id != oppId)
                return BadRequest("Opportunity Id does not match with request id");
            var result = await _opportunitiesService.UpdateOpportunityAsync(opp);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission. 
        /// </summary>
        /// <param name="oppId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpDelete]
        [Route("opportunities/{oppId}")]
        public async Task<IHttpActionResult> DeleteOpportunity(string oppId)
        {
            await _opportunitiesService.DeleteOpportunityAsync(oppId);
            return Ok();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="oppId"></param>
        /// <param name="skill"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("opportunities/{oppId}/AddSkill")]
        public async Task<IHttpActionResult> AddSkill(string oppId, [FromBody] Skill skill)
        {
            var permissionTypeCreated = await _opportunitiesService.AddSkillAsync(oppId, skill);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionTypeCreated.Id}";
            var response = Created(create, permissionTypeCreated);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oppId"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("opportunities/{oppId}/RemoveSkill/{skillId}")]
        public async Task<IHttpActionResult> RemoveSkill(string oppId, string skillId)
        {
            var permissionTypeCreated = await _opportunitiesService.RemoveSkillAsync(oppId, skillId);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionTypeCreated.Id}";
            var response = Created(create, permissionTypeCreated);
            return response;
        }
    }
}
