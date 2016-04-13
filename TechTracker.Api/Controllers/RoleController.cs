using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using TechTracker.Api.Common.Attributes;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Services.Interfaces;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// Controller to manage the roles by tenant
    /// </summary>
    [RoutePrefix("api")]
    public class RoleController : AbstractBaseController
    {
        private readonly IRolesService _rolesService;
        private readonly IUserProfileService _userProfileService;
        private readonly IPermissionsService _permissionsService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="rolesService">Roles service to be injected</param>
        /// <param name="userProfileService"></param>
        /// <param name="permissionsService"></param>
        public RoleController(ILog logger, IRolesService rolesService, IUserProfileService userProfileService, IPermissionsService permissionsService) : base(logger)
        {
            _rolesService = rolesService;
            _userProfileService = userProfileService;
            _permissionsService = permissionsService;
        }

        /// <summary>
        /// POST endpoint to add a new role to a tenant
        /// </summary>
        /// <param name="role">The role object to add</param>
        /// <returns>201 Created response and a header with the location of the new role created</returns>
        [HttpPost]
        [Route("roles")]
        public async Task<IHttpActionResult> AddRoleAsync(Role role)
        {
            var addedRole = await _rolesService.AddRoleAsync(role, UserName);

            if (HttpContext.Current != null)
            {
                var create = HttpContext.Current.Request.Url + $"/{addedRole.Id}";
                var response = Created(create, addedRole);
                return response;
            }
            return Created("", addedRole);
        }

        /// <summary>
        /// GET endpoint to retrieve roles
        /// </summary>
        /// <returns>Returns all the roles for current Tenant</returns>
        [HttpGet]
        [Route("roles")]
        public async Task<IHttpActionResult> GetRolesAsync()
        {
            var result = await _rolesService.GetAsync();
            return Ok(result);
        }

        /// <summary>
        /// GET endpoint to retrieve a role from a tenant
        /// </summary>
        /// <param name="roleId">The Id of the role</param>
        /// <returns>The role that matches the given Id</returns>
        [HttpGet]
        [Route("roles/{roleId}")]
        public async Task<IHttpActionResult> GetRoleAsync(string roleId)
        {
            var result = await _rolesService.GetDtoByIdAsync(roleId);
            return Ok(result);
        }

        /// <summary>
        /// PUT endpoint to update an existing role into a tenant
        /// </summary>
        /// <param name="roleId">The Id of the role to update</param>
        /// <param name="role">The object of the new role body</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpPut]
        [Route("roles/{roleId}")]
        public async Task<IHttpActionResult> UpdateRoleAsync(string roleId, [FromBody] Role role)
        {
            try
            {
                if (role.Id != roleId)
                    return BadRequest("Role Id does not match with request id");

                var rrole = _rolesService.GetByIdAsync(roleId);
                if (rrole == null)
                    return BadRequest("Requested role not found");

                await _rolesService.UpdateRoleAsync(role, UserName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// DELETE endpoint to delete and existing role from a tenant
        /// </summary>
        /// <param name="roleId">The Id of the role to delete</param>
        /// <returns>200 Ok status if everything went well</returns>
        [HttpDelete]
        [Route("roles/{roleId}")]
        public IHttpActionResult DeleteRole(string roleId)
        {
            try
            {
                var role = _rolesService.GetByIdAsync(roleId);
                if (role == null)
                    return BadRequest("Requested role not found");

                _rolesService.DeleteRoleAsync(roleId, UserName);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// POST endpoint to add a permission to a Tenant role
        /// </summary>
        /// <param name="roleId">The Id of the role to update</param>
        /// <param name="permId">The Id of the permission to add</param>
        /// <returns>The updated role object</returns>
        [HttpPost]
        [Route("roles/{roleId}/Permissions/Add/{permId}")]
        public async Task<IHttpActionResult> AddPermissionToRoleAsync(string roleId, string permId)
        {
            try
            {
                var permissionExists = await _permissionsService.GetPermissionAsync(permId);
                if (permissionExists == null)
                    return BadRequest("Requested permission not found");
                //var result = await _rolesService.AddPermissionToRoleAsync(roleId, permId, TenantId, UserName);
                var result = await _rolesService.AddPermissionToRoleAsync(roleId, permId, UserName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// POST endpoint to delete a permission from a Tenant role
        /// </summary>
        /// <param name="roleId">The Id of the role to update</param>
        /// <param name="permId">The Id of the permission to remove</param>
        /// <returns>The updated role object</returns>
        [HttpPost]
        [Route("roles/{roleId}/Permissions/Remove/{permId}")]
        public async Task<IHttpActionResult> RemovePermissionFromRoleAsync(string roleId, string permId)
        {
            try
            {
                var permissionExists = await _permissionsService.GetPermissionAsync(permId);
                if (permissionExists == null)
                    return BadRequest("Requested permission not found");
                var result = await _rolesService.DeletePermissionFromRoleAsync(roleId, permId, UserName);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// POST endpoint to add a user to a Tenant role
        /// </summary>
        /// <param name="roleId">The Id of the role to update</param>
        /// <param name="userId">The Id of the user to add</param>
        /// <returns>The updated role object</returns>
        [HttpPost]
        [Route("roles/{roleId}/Users/Add/{userId}")]
        public async Task<IHttpActionResult> AddUserToARoleAsync(string roleId, string userId)
        {
            try
            {
                var userExists = await _userProfileService.GetUser(userId);
                if (userExists == null)
                    return BadRequest("Requested user not found");

                var result = await _rolesService.AddUserToRoleAsync(roleId, userId, UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        /// <summary>
        /// POST endpoint to remove a user from a Tenant role
        /// </summary>
        /// <param name="roleId">The Id of the role to update</param>
        /// <param name="userId">The Id of the user to remove</param>
        /// <returns>The updated role object</returns>
        [HttpPost]
        [Route("roles/{roleId}/Users/Remove/{userId}")]
        public async Task<IHttpActionResult> RemoveUserToARoleAsync(string roleId, string userId)
        {
            try
            {
                var userExists = _userProfileService.GetUser(userId);
                if (userExists == null)
                    return BadRequest("Requested user not found");

                var result = await _rolesService.RemoveUserFromRoleAsync(roleId, userId, UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("roles/Users/{userId}")]
        public async Task<IHttpActionResult> GetUserRoles(string userId)
        {
            var result = await _rolesService.GetRolesByUserIdAsync(userId);
            return Ok(result);
        }

        
    }
}
