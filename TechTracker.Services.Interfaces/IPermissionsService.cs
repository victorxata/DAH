using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.RBAC;

namespace TechTracker.Services.Interfaces
{
    public interface IPermissionsService
    {
        Permission AddPermission(Permission permission);
        Task<Permission> AddPermissionAsync(Permission permission);

        Permission UpdatePermission(Permission permission);
        Task<Permission> UpdatePermissionAsync(Permission permission);

        Permission GetPermission(string permId);
        Task<Permission> GetPermissionAsync(string permId);

        void DeletePermission(string permId);
        Task DeletePermissionAsync(string permId);

        IEnumerable<Permission> GetPermissions();

        Task<IEnumerable<Permission>> GetPermissionsAsync();
    }
}
