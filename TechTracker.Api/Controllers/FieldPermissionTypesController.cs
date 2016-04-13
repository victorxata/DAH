using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using log4net;
using TechTracker.Api.Common.Attributes;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Services.Interfaces;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Validation]
    [RoutePrefix("api")]
    public class FieldPermissionTypesController : BaseController
    {
        private readonly IFieldPermissionTypesService _fieldPermissionTypesService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fieldPermissionTypesService"></param>
        public FieldPermissionTypesController(ILog logger, IFieldPermissionTypesService fieldPermissionTypesService) : base(logger)
        {
            _fieldPermissionTypesService = fieldPermissionTypesService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the field permission types with all the properties that can be securized
        /// </summary>
        /// <returns>The complete list of field permission types</returns>
        [Authorize]
        [HttpGet]
        [Route("fieldPermissionTypes")]
        public async Task<IHttpActionResult> GetFieldPermissionTypes()
        {
            var result = await _fieldPermissionTypesService.GetFieldPermissionTypesAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve a field permission type
        /// </summary>
        /// <param name="fieldPermissionTypeId">The Id of the field permission type</param>
        /// <returns>The field permission type that matches the given Id</returns>
        [Authorize]
        [HttpGet]
        [Route("fieldPermissionTypes/{fieldPermissionTypeId}")]
        public async Task<IHttpActionResult> GetFieldPermissionTypeById(string fieldPermissionTypeId)
        {
            var result = await _fieldPermissionTypesService.GetFieldPermissionTypeByIdAsync(fieldPermissionTypeId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new field permission type. Can be executed only by people in the Main Roles collection "Admin"
        /// </summary>
        /// <param name="fieldPermissionType">The field permission type object to add</param>
        /// <returns>201 Created response and a header with the location of the new field permission type created</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("fieldPermissionTypes")]
        public async Task<IHttpActionResult> PostFieldPermissionType([FromBody] FieldPermissionType fieldPermissionType)
        {
            var permissionTypeCreated = await _fieldPermissionTypesService.AddFieldPermissionTypeAsync(fieldPermissionType);

            if (HttpContext.Current == null)
                return Created("", permissionTypeCreated);

            var create = HttpContext.Current.Request.Url + String.Format("/{0}", permissionTypeCreated.Id);
            var response = Created(create, permissionTypeCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing field permission type. Can be executed only by people in the Main Roles collection "Admin"
        /// </summary>
        /// <param name="fieldPermissionTypeId">The Id of the field permission type to retrieve</param>
        /// <param name="fieldPermissionType">The object of the new field permission type body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("fieldPermissionTypes/{fieldPermissionTypeId}")]
        public async Task<IHttpActionResult> UpdateFieldPermissionType(string fieldPermissionTypeId, [FromBody] FieldPermissionType fieldPermissionType)
        {
            if (fieldPermissionType.Id != fieldPermissionTypeId)
                return BadRequest("Field Permission Id does not match with request id");
            var result = await _fieldPermissionTypesService.UpdateFieldPermissionTypeAsync(fieldPermissionType);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission. Can be executed only by people in the Main Roles collection "Admin"
        /// </summary>
        /// <param name="fieldPermissionTypeId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("fieldPermissionTypes/{fieldPermissionTypeId}")]
        public async Task<IHttpActionResult> DeleteFieldPermissionType(string fieldPermissionTypeId)
        {
            await _fieldPermissionTypesService.DeleteFieldPermissionTypeAsync(fieldPermissionTypeId);
            return Ok();
        }

    }
}
