using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Identity;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class RecoverPasswordRepository : MongoRepository<RecoverPassword>, IRecoverPasswordRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<RecoverPassword> col)
        {
            var userName = Builders<RecoverPassword>.IndexKeys.Ascending(t => t.UserName);
            var unique = new CreateIndexOptions { Unique = false, Sparse = true};
            col.Indexes.CreateOneAsync(userName, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.RecoverPasswords;
    }
}
