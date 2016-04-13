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
    public class SkillsController : BaseController
    {
        private readonly ISkillsService _skillsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="skillsService"></param>
        public SkillsController(ILog logger, ISkillsService skillsService) : base(logger)
        {
            _skillsService = skillsService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the skills with all the properties that can be securized
        /// </summary>
        /// <returns>The complete list of skills</returns>
        [HttpGet]
        [Route("skills")]
        public async Task<IHttpActionResult> GetSkills()
        {
            var result = await _skillsService.GetSkillAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve a skill
        /// </summary>
        /// <param name="skillId">The Id of the skill</param>
        /// <returns>The skill that matches the given Id</returns>
        [HttpGet]
        [Route("skills/{skillId}")]
        public async Task<IHttpActionResult> GetSkillById(string skillId)
        {
            var result = await _skillsService.GetSkillByIdAsync(skillId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new skill. 
        /// </summary>
        /// <param name="skill">The skill object to add</param>
        /// <returns>201 Created response and a header with the location of the new skill created</returns>
        [HttpPost]
        [Route("skills")]
        public async Task<IHttpActionResult> PostSkill([FromBody] Skill skill)
        {
            var permissionTypeCreated = await _skillsService.AddSkillAsync(skill);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionTypeCreated.Id}";
            var response = Created(create, permissionTypeCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing skill.
        /// </summary>
        /// <param name="skillId">The Id of the skill to retrieve</param>
        /// <param name="skill">The object of the new skill body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpPut]
        [Route("skills/{skillId}")]
        public async Task<IHttpActionResult> UpdateSkill(string skillId, [FromBody] Skill skill)
        {
            if (skill.Id != skillId)
                return BadRequest("Field Permission Id does not match with request id");
            var result = await _skillsService.UpdateSkillAsync(skill);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission. 
        /// </summary>
        /// <param name="skillId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpDelete]
        [Route("skills/{skillId}")]
        public async Task<IHttpActionResult> DeleteSkill(string skillId)
        {
            await _skillsService.DeleteSkillAsync(skillId);
            return Ok();
        }

    }
}
