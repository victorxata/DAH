using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class PermissionsRepository : MongoRepository<Permission>, IPermissionsRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<Permission> col)
        {
            var endPoint = Builders<Permission>.IndexKeys.Ascending(t => t.Endpoint);
            var unique = new CreateIndexOptions { Unique = true };
            col.Indexes.CreateOneAsync(endPoint, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.Permissions;
    }
}
