using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class CandidatesRepository : MongoRepository<Candidate>, ICandidatesRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<Candidate> col)
        {
            var desc = Builders<Candidate>.IndexKeys.Ascending(t => t.Name);
            var unique = new CreateIndexOptions { Unique = true, Sparse = true};
            col.Indexes.CreateOneAsync(desc, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.Candidates;
    }
}
