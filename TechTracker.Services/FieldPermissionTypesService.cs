using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class FieldPermissionTypesService : IFieldPermissionTypesService
    {
        private readonly IFieldPermissionTypesRepository _fieldPermissionTypesRepository;

        private readonly ILog Log;

        public FieldPermissionTypesService(IFieldPermissionTypesRepository fieldPermissionTypesRepository, ILog log)
        {
            _fieldPermissionTypesRepository = fieldPermissionTypesRepository;
            Log = log;
        }

        public async Task<IEnumerable<FieldPermissionType>> GetFieldPermissionTypesAsync()
        {
            return await _fieldPermissionTypesRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<FieldPermissionType> GetFieldPermissionTypeByIdAsync(string fieldPermissionTypeId)
        {
            return await _fieldPermissionTypesRepository.GetByIdAsync(fieldPermissionTypeId);
        }

        public async Task<FieldPermissionType> AddFieldPermissionTypeAsync(FieldPermissionType fieldPermissionType)
        {
            return await _fieldPermissionTypesRepository.AddAsync(fieldPermissionType);
        }

        public async Task<FieldPermissionType> UpdateFieldPermissionTypeAsync(FieldPermissionType fieldPermissionType)
        {
            return await _fieldPermissionTypesRepository.UpdateAsync(fieldPermissionType);
        }

        public async Task DeleteFieldPermissionTypeAsync(string fieldPermissionTypeId)
        {
            await _fieldPermissionTypesRepository.DeleteAsync(fieldPermissionTypeId);
        }
    }
}
