using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class RolesRepository : MongoRepository<Role>, IRolesRepository
    {
        
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<Role> col)
        {
            var roleName = Builders<Role>.IndexKeys.Ascending(t => t.Name);
            var unique = new CreateIndexOptions { Unique = true };
            col.Indexes.CreateOneAsync(roleName, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.CustomRoles;
    }
}
