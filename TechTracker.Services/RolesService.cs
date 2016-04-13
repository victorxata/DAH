using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Domain.Data.Identity;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _rolesRepository;
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IPermissionsService _permissionsService;

        private readonly ILog Log;

        public RolesService(IRolesRepository rolesRepository, IPermissionsRepository permissionsRepository, IPermissionsService permissionsService, ILog log)
        {
            _rolesRepository = rolesRepository;
            _permissionsRepository = permissionsRepository;
            _permissionsService = permissionsService;
            Log = log;
        }

        public async Task<Role> AddRoleAsync(Role role, string userName)
        {
            return await _rolesRepository.AddAsync(role);
        }

        public async Task<Role> DeletePermissionFromRoleAsync(string roleId, string permId, string userName)
        {
            var role = await GetByIdAsync(roleId);

            if (role.PermissionNames.Any(p => p == permId))
            {
                role.PermissionNames.Remove(permId);
                return await _rolesRepository.UpdateAsync(role);
            }
            return role;
        }

        public async Task<Role> AddPermissionToRoleAsync(string roleId, string permId, string userName)
        {
            var role = await GetByIdAsync(roleId);

            if (role.PermissionNames.All(p => p != permId))
            {
                role.PermissionNames.Add(permId);
                return await _rolesRepository.UpdateAsync(role);
            }
            return role;
        }
        
        public async Task<Role> AddUserToRoleAsync(Role role, string userId, string userName)
        {
            if (!role.UserIds.Contains(userId))
            {
                role.UserIds.Add(userId);
                return await _rolesRepository.UpdateAsync(role);
            }

            throw new Exception("Role already contains user " + userId);
        }

        public async Task<Role> AddUserToRoleAsync(string roleId, string userId, string userName)
        {
            var role = await GetByIdAsync(roleId);
            var result = await AddUserToRoleAsync(role, userId,userName);
            return result;
        }

        public async Task<bool> UserIsSuperUserAsync(string userId)
        {
            var collection = _rolesRepository.Collection;

            var userRoles = await collection.CountAsync(x => x.UserIds.Contains(userId) && x.Name == Role.SuperUserName);

            return userRoles > 0;
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(string userId)
        {
            var collection = _rolesRepository.Collection;

            //1)  get all roles for user
            var usersRoles = await collection.FindAsync(r => r.UserIds.Contains(userId));

            // 2a) get the orgunit the user lives in
            // TODO: Review why it fails
            //var orgUnitsService = new OrgUnitsService(tenantId);
            // usersOrgUnit = orgUnitsService.GetUsersOrgUnit(userId);

            // 2b) get all roles for user's orgunit
            //var ouRoles = collection.Find(x => x.OrgUnitIds.Contains(usersOrgUnit.CorrelationId)).ToListAsync().Result;
            //var ousRoles = new List<Role>();

            //TODO: This code commented due it seems to do nothing
            //if (usersOrgUnit != null)
            //{
            //    ouRoles.Where(r => r.OrgUnitIds.Contains(usersOrgUnit.CorrelationId));
            //}

            // 3)  get all permissions for roles
            var permissions = await _permissionsRepository.Collection.Find(new BsonDocument()).ToListAsync();

            var userPermissions = new List<Permission>();

            foreach (var role in usersRoles.ToListAsync().Result)   //.Concat(ousRoles.ToList()))
            {
                foreach (var permissionName in role.PermissionNames)
                {
                    var permission = permissions.FirstOrDefault(p => p.Id == permissionName);

                    if (permission == null) continue;

                    if (!userPermissions.Contains(permission))
                    {
                        userPermissions.Add(permission);
                    }
                }
            }

            return userPermissions;
        }

        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(string userId)
        {
            var roles = await _rolesRepository.Collection.Find(x => x.UserIds.Contains(userId)).ToListAsync();

            return roles;
        }

        public async Task<Role> RemoveUserFromRoleAsync(string roleId, string userId, string userName)
        {
            var role = await GetByIdAsync(roleId);
            if (role.UserIds.Contains(userId))
            {
                role.UserIds.Remove(userId);
                return await _rolesRepository.UpdateAsync(role);
            }

            throw new Exception("Role does not contains user " + userId);
        }

        public async Task<IEnumerable<Role>> GetAsync()
        {
             return await _rolesRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }

        private async Task<RoleDto> BuildRoleAsync(Role role)
        {
            var permissions = await _permissionsService.GetPermissionsAsync();
            var permissionsList = permissions as IList<Permission> ?? permissions.ToList();
            var permiss = role.PermissionNames.Select(permissionName => permissionsList.FirstOrDefault(x => x.Id == permissionName)).ToList();
            var users = new List<UserDto>();
            foreach (var userId in role.UserIds)
            {
                var user = await Managers.UsersCollection.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user != null)
                    users.Add(new UserDto
                    {
                        Id = user.Id,
                        Username = user.RealName ?? user.UserName
                    });
            }

            return new RoleDto
            {
                Name = role.Name,
                Permissions = permiss,
                Users = users
            };
        }

        public async Task<Role> GetByIdAsync(string roleId)
        {
            var role = await _rolesRepository.GetByIdAsync(roleId);
            return role;
        }

        public async Task<RoleDto> GetDtoByIdAsync(string roleId)
        {
            
            var role = await _rolesRepository.GetByIdAsync(roleId);
            return await BuildRoleAsync(role);
        }

        public async Task<Role> UpdateRoleAsync(Role role, string userName)
        {
            var result = await _rolesRepository.UpdateAsync(role);
            return result;
        }

        public async Task DeleteRoleAsync(string roleId, string userName)
        {
            await _rolesRepository.DeleteAsync(x => x.Id == roleId);
        }
    }
}
