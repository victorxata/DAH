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
    public class PermissionsService : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository;

        private readonly ILog Log;

        #region Constructors

        public PermissionsService(IPermissionsRepository permissionsRepository, ILog log)
        {
            _permissionsRepository = permissionsRepository;
            Log = log;
        }

        #endregion

        public Permission AddPermission(Permission permission)
        {
            _permissionsRepository.AddAsync(permission).Wait();
            return permission;
        }

        public async Task<Permission> AddPermissionAsync(Permission permission)
        {
            var perm = await _permissionsRepository.AddAsync(permission);

            return perm;
        }

        public Permission UpdatePermission(Permission permission)
        {
            _permissionsRepository.UpdateAsync(permission).Wait();
                return permission;
        }

        public async Task<Permission> UpdatePermissionAsync(Permission permission)
        {
            return await _permissionsRepository.UpdateAsync(permission);
        }

        public Permission GetPermission(string permId)
        {
            var perm = _permissionsRepository.GetByIdAsync(permId).Result;
                return perm;
        }

        public async Task<Permission> GetPermissionAsync(string permId)
        {
            return await _permissionsRepository.GetByIdAsync(permId);
        }

        public void DeletePermission(string permId)
        {
            _permissionsRepository.DeleteAsync(permId).Wait();
        }

        public async Task DeletePermissionAsync(string permId)
        {
            await _permissionsRepository.DeleteAsync(permId);
        }

        public IEnumerable<Permission> GetPermissions()
        {
            var cursor = _permissionsRepository.Collection.Find(new BsonDocument()).ToListAsync().Result;

                return cursor.ToList();
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return await _permissionsRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }
    }
}
