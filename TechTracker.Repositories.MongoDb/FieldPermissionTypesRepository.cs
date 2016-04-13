using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class FieldPermissionTypesRepository : MongoRepository<FieldPermissionType>, IFieldPermissionTypesRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<FieldPermissionType> col)
        {
            var fptName = Builders<FieldPermissionType>.IndexKeys.Ascending(t => t.TypeName);
            var unique = new CreateIndexOptions { Unique = true, Sparse = true};
            col.Indexes.CreateOneAsync(fptName, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.FieldPermissionTypes;
    }
}
