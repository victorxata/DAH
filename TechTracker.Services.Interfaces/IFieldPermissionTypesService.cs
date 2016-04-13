using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.RBAC;

namespace TechTracker.Services.Interfaces
{
    public interface IFieldPermissionTypesService
    {
        Task<IEnumerable<FieldPermissionType>> GetFieldPermissionTypesAsync();
        
        Task<FieldPermissionType> GetFieldPermissionTypeByIdAsync(string fieldPermissionTypeId);
        
        Task<FieldPermissionType> AddFieldPermissionTypeAsync(FieldPermissionType fieldPermissionType);
       
        Task<FieldPermissionType> UpdateFieldPermissionTypeAsync(FieldPermissionType fieldPermissionType);
        
        Task DeleteFieldPermissionTypeAsync(string fieldPermissionTypeId);
    }
}
