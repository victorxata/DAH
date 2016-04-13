using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Identity;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class PreRegistrationRepository : MongoRepository<PreRegisteredUser>, IPreRegistrationRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<PreRegisteredUser> col)
        {
            var userName = Builders<PreRegisteredUser>.IndexKeys.Ascending(t => t.UserName);
            var opt = new CreateIndexOptions { Unique = false, Sparse = true};
            col.Indexes.CreateOneAsync(userName, opt);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.Preregistrations;
    }
}
