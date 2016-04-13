using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.RBAC;

namespace TechTracker.Services.Interfaces
{
    /// <summary>
    /// Service to manage roles for a given Tenant
    /// </summary>
    public interface IRolesService
    {
        /// <summary>
        /// Async list of current Roles
        /// </summary>
        /// <returns>Returns a List of Roles</returns>
        Task<IEnumerable<Role>>  GetAsync();

        

        /// <summary>
        /// Async Adds a new role
        /// </summary>
        /// <param name="role">The Role object to add</param>
        /// <param name="userName">The username that launches the operation</param>
        /// <returns>The updated Role</returns>
        Task<Role> AddRoleAsync(Role role, string userName);

        /// <summary>
        /// Async get a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <returns>A role object</returns>
        Task<Role> GetByIdAsync(string roleId);
        
        /// <summary>
        /// Async get a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <returns>A role object</returns>
        Task<RoleDto> GetDtoByIdAsync(string roleId);

        /// <summary>
        /// Async updates an existing Role 
        /// </summary>
        /// <param name="role">The new role object</param>
        /// <param name="userName"></param>
        /// <returns>The updated role object</returns>
        Task<Role> UpdateRoleAsync(Role role, string userName);

        /// <summary>
        /// Async deletes a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <param name="userName"></param>
        /// <returns>Http status</returns>
        Task DeleteRoleAsync(string roleId, string userName);

        /// <summary>
        /// Async delete a permission from a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <param name="permId">The CorrelationId of the Permission</param>
        /// <param name="userName">The username that launches the operation</param>
        /// <returns>The updated Role object</returns>
        Task<Role> DeletePermissionFromRoleAsync(string roleId, string permId, string userName);
        
        /// <summary>
        /// Async add a permission to a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <param name="permId">The CorrelationId of the Permission</param>
        /// <param name="userName">The username that launches the operation</param>
        /// <returns>The updated Role object</returns>
        Task<Role> AddPermissionToRoleAsync(string roleId, string permId, string userName);
        
        /// <summary>
        /// Async add a user to a role
        /// </summary>
        /// <param name="role">The role object</param>
        /// <param name="userId">The CorrelationId of the user to add</param>
        /// <param name="userName">The username that launches the operation</param>
        /// <returns>The updated Role object</returns>
        Task<Role> AddUserToRoleAsync(Role role, string userId, string userName);

        /// <summary>
        /// Async add a user to a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <param name="userId">The CorrelationId of the user to add</param>
        /// <param name="userName">The username that launches the operation</param>
        /// <returns>The updated Role object</returns>
        Task<Role> AddUserToRoleAsync(string roleId, string userId, string userName);

        /// <summary>
        /// Check if the given user is a superuser in the given tenant
        /// </summary>
        /// <param name="userId">The CorrelationId of the user</param>
        /// <returns></returns>
        Task<bool> UserIsSuperUserAsync(string userId);

        /// <summary>
        /// Returns the permissions of the given user in the given tenant
        /// </summary>
        /// <param name="userId">The CorrelationId of the user</param>
        /// <returns>A list of Permissions</returns>
        Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(string userId);

        /// <summary>
        /// Returns the roles of the given user in the given tenant
        /// </summary>
        /// <param name="userId">The CorrelationId of the user</param>
        /// <returns>A list of roles</returns>
        Task<IEnumerable<Role>> GetRolesByUserIdAsync(string userId);

        /// <summary>
        /// Async removes a user from a role
        /// </summary>
        /// <param name="roleId">The CorrelationId of the Role</param>
        /// <param name="userId">The CorrelationId of the user</param>
        /// <param name="userName">The username that launches the operation</param>
        /// <returns>The updated Role object</returns>
        Task<Role> RemoveUserFromRoleAsync(string roleId, string userId, string userName);

        
    }
}
