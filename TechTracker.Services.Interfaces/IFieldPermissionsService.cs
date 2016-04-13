using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.RBAC;

namespace TechTracker.Services.Interfaces
{
    public interface IFieldPermissionsService
    {
        Task<FieldPermission> AddFieldPermissionAsync(FieldPermission fieldPermission, string userName);

        Task<FieldPermission> UpdateFieldPermissionAsync(FieldPermission fieldPermission, string userName);

        Task<FieldPermission> GetFieldPermissionByIdAsync(string fieldPermId);

        Task DeleteFieldPermissionAsync(string fieldPermId, string userName);

        Task<List<FieldPermission>> GetFieldPermissionsAsync(string className);
        Task<List<FieldPermission>> GetFieldPermissionsAsync(string userId, string userName);
        Task<List<FieldPermission>> GetFieldPermissionsAsync();

    }
}
