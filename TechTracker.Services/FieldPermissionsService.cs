using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class FieldPermissionsService : IFieldPermissionsService
    {
        private readonly IFieldPermissionsRepository _fieldPermissionsRepository;

        private readonly IRolesService _rolesService;

        private readonly ILog Log;

        public FieldPermissionsService(IFieldPermissionsRepository fieldPermissionsRepository, IRolesService rolesService, ILog log)
        {
            _fieldPermissionsRepository = fieldPermissionsRepository;
            _rolesService = rolesService;
            Log = log;
        }

        public async Task<FieldPermission> AddFieldPermissionAsync(FieldPermission fieldPermission, string userName)
        {
            return await _fieldPermissionsRepository.AddAsync(fieldPermission);
        }

        public async Task<FieldPermission> UpdateFieldPermissionAsync(FieldPermission fieldPermission, string userName)
        {
            var result = await _fieldPermissionsRepository.UpdateAsync(fieldPermission);
            return result;
        }

        public async Task<FieldPermission> GetFieldPermissionByIdAsync(string permId)
        {
            var fieldPermission = await _fieldPermissionsRepository.GetByIdAsync(permId);
            return fieldPermission;
        }

        public async Task DeleteFieldPermissionAsync(string permId, string userName)
        {
            await _fieldPermissionsRepository.DeleteAsync(x => x.Id == permId);
        }

        public async Task<List<FieldPermission>> GetFieldPermissionsAsync(string className)
        {
            var filter = Builders<FieldPermission>.Filter;
            var list = _fieldPermissionsRepository.Collection;
            var query = filter.Eq(x => x.ClassName, className);

            var permissions = await list.Find(query).ToListAsync();

            return permissions;
        }

        public async Task<List<FieldPermission>> GetFieldPermissionsAsync(string userId, string userName)
        {
            var list = _fieldPermissionsRepository.Collection;

            var roles = await _rolesService.GetRolesByUserIdAsync(userId); 
            var rls = roles.Select(x => x.Name).ToArray();
            var filter = new BsonDocument("$or", new BsonArray
            {
                new BsonDocument("userName", userName),
                new BsonDocument("roleName", new BsonDocument("$in", new BsonArray(rls.ToArray())))
            });
            var permissions = await list.Find(filter).ToListAsync();
            
            return permissions;
        }

        public async Task<List<FieldPermission>> GetFieldPermissionsAsync()
        {
            return await _fieldPermissionsRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }
    }
}
