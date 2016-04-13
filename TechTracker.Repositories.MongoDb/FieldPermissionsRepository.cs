using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class FieldPermissionsRepository : MongoRepository<FieldPermission>, IFieldPermissionsRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<FieldPermission> col)
        {
            var className = Builders<FieldPermission>.IndexKeys.Ascending(t => t.ClassName);
            var unique = new CreateIndexOptions { Unique = false };
            col.Indexes.CreateOneAsync(className, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.FieldPermissions;
    }
}
