using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using log4net;
using Microsoft.AspNet.Identity;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Services.Interfaces;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api")]
    public class FieldPermissionsController : AbstractBaseController
    {
        private readonly IFieldPermissionsService _fieldPermissionsService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="fieldPermissionsService"></param>
        public FieldPermissionsController(ILog logger, IFieldPermissionsService fieldPermissionsService) : base(logger)
        {
            _fieldPermissionsService = fieldPermissionsService;
        }

        /// <summary>
        /// GET endpoint to retrieve all the field permissions based on the class name and the current Tenant
        /// </summary>
        /// <param name="className">The class name</param>
        /// <returns>All the field permissions that matches the class name</returns>
        [HttpGet]
        [Route("fieldPermissions/ByClass/{className}")]
        public async Task<IHttpActionResult> GetFieldPermissionsByField(string className)
        {
            var result = await _fieldPermissionsService.GetFieldPermissionsAsync(className);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve all the field permissions based on the current user and Tenant
        /// </summary>
        /// <returns>All the field permissions that matches the current user, the roles that the user are in, and the current Tenant</returns>
        [HttpGet]
        [Route("fieldPermissions/ByCurrentUser")]
        public IHttpActionResult GetFieldPermissionsByTenantAndUser()
        {
            var result = _fieldPermissionsService.GetFieldPermissionsAsync(User.Identity.GetUserId(), UserName);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve all the permissions based on a Tenant
        /// </summary>
        /// <returns>All the field permissions that matches the given tenant</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("fieldPermissions")]
        public async Task<IHttpActionResult> GetAllFieldPermissionsByTenant()
        {
            var result = await _fieldPermissionsService.GetFieldPermissionsAsync(TenantId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve a field permission
        /// </summary>
        /// <param name="fieldPermissionId">The Id of the field permission</param>
        /// <returns>The field permission that matches the given Id</returns>
        [HttpGet]
        [Route("fieldPermissions/{fieldPermissionId}")]
        public async Task<IHttpActionResult> GetFieldPermissionById(string fieldPermissionId)
        {
            var result = await _fieldPermissionsService.GetFieldPermissionByIdAsync(fieldPermissionId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new permission
        /// </summary>
        /// <param name="fieldPermission">The permission object to add</param>
        /// <returns>201 Created response and a header with the location of the new permission created</returns>
        [HttpPost]
        [Route("fieldPermissions")]
        public async Task<IHttpActionResult> PostPermission([FromBody] FieldPermission fieldPermission)
        {
            var permissionCreated = await _fieldPermissionsService.AddFieldPermissionAsync(fieldPermission, UserName);

            if (HttpContext.Current == null)
                return Created("", permissionCreated);

            var create = HttpContext.Current.Request.Url + $"/{permissionCreated.Id}";
            var response = Created(create, permissionCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing permission
        /// </summary>
        /// <param name="fieldPermissionId"></param>
        /// <param name="fieldPermission">The object of the new permission body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpPut]
        [Route("fieldPermissions/{fieldPermissionId}")]
        public async Task<IHttpActionResult> UpdatePermission(string fieldPermissionId, [FromBody] FieldPermission fieldPermission)
        {
            if (fieldPermission.Id != fieldPermissionId)
                return BadRequest("Field Permission Id does not match with request id");
            var result = await _fieldPermissionsService.UpdateFieldPermissionAsync(fieldPermission, UserName);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission
        /// </summary>
        /// <param name="fieldPermissionId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpDelete]
        [Route("fieldPermissions/{fieldPermissionId}")]
        public async Task<IHttpActionResult> DeletePermission(string fieldPermissionId)
        {
            await _fieldPermissionsService.DeleteFieldPermissionAsync(fieldPermissionId, UserName);
            return Ok();
        }

    }
}
