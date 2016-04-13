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
    /// This controller will manage the permissions for the whole application
    /// </summary>
    
    [Validation]
    [RoutePrefix("api")]
    public class PermissionsController : BaseController
    {
        private readonly IPermissionsService _permissionsService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="permissionsService"></param>
        public PermissionsController(ILog logger, IPermissionsService permissionsService) : base(logger)
        {
            _permissionsService = permissionsService;
        }

        /// <summary>
        /// GET endpoint to retrieve permissions
        /// </summary>
        /// <returns>Returns all the permissions</returns>
        [Authorize]
        [HttpGet]
        [Route("permissions")]
        public async Task<IHttpActionResult> GetPermissions()
        {
            var result = await _permissionsService.GetPermissionsAsync();
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// GET endpoint to retrieve a permission
        /// </summary>
        /// <param name="permId">The Id of the permission</param>
        /// <returns>The permission that matches the given Id</returns>
        [Authorize]
        [HttpGet]
        [Route("permissions/{permId}")]
        public async Task<IHttpActionResult> GetPermission(string permId)
        {
            var result = await _permissionsService.GetPermissionAsync(permId);
            if (result != null)
            {
                return Ok(result);
            }
            return new StatusCodeResult(HttpStatusCode.NoContent, Request);
        }

        /// <summary>
        /// POST endpoint to add a new permission. Can be executed only by people in the Main Roles collection "Admin"
        /// </summary>
        /// <param name="permission">The permission object to add</param>
        /// <returns>201 Created response and a header with the location of the new permission created</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("permissions")]
        public async Task<IHttpActionResult> PostPermission([FromBody] Permission permission)
        {
            var permissionCreated = await _permissionsService.AddPermissionAsync(permission);

            if (HttpContext.Current == null) 
                return Created("", permissionCreated);

            var create = HttpContext.Current.Request.Url + String.Format("{0}/{1}", "permissions", permissionCreated.Id);
            var response = Created(create, permissionCreated);
            return response;
        }

        /// <summary>
        /// PUT endpoint to update an existing permission. Can be executed only by people in the Main Roles collection "Admin"
        /// </summary>
        /// <param name="permId">The Id of the permission to update</param>
        /// <param name="permission">The object of the new permission body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("permissions/{permId}")]
        public async Task<IHttpActionResult> UpdatePermission(string permId, [FromBody] Permission permission)
        {
            if (permission.Id != permId)
                return BadRequest("Permission Id does not match with request id");
            var result = await _permissionsService.UpdatePermissionAsync(permission);
            return Ok(result);
        }

        /// <summary>
        /// DELETE endpoint to delete and existing permission. Can be executed only by people in the Main Roles collection "Admin"
        /// </summary>
        /// <param name="permId">The Id of the permission to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("permissions/{permId}")]
        public async Task<IHttpActionResult> DeletePermission(string permId)
        {
            await _permissionsService.DeletePermissionAsync(permId);
            return Ok();
        }

        
    }
}
