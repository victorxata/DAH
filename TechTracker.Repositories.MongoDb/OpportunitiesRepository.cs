using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class OpportunitiesRepository : MongoRepository<Opportunity>, IOpportunitiesRepository
    {
        private readonly ISummaryRepository _summaryRepository;
        private readonly IAccountsRepository _accountsRepository;

        public OpportunitiesRepository(ISummaryRepository summaryRepository, IAccountsRepository accountsRepository)
        {
            _summaryRepository = summaryRepository;
            _accountsRepository = accountsRepository;
        }

        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<Opportunity> col)
        {
            var desc = Builders<Opportunity>.IndexKeys.Ascending(t => t.Role);
            var unique = new CreateIndexOptions { Unique = true, Sparse = true};
            col.Indexes.CreateOneAsync(desc, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.Opportunities;

        public override async Task<Opportunity> AddAsync(Opportunity entity)
        {
            var summary = await _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year)).FirstOrDefaultAsync()
                          ??
                          new Summary { Year = DateTime.UtcNow.Year };

            await base.AddAsync(entity);

            var account = await _accountsRepository.GetByIdAsync(entity.AccountId);

            summary = summary.AddRole(entity.Id, entity.AccountId, account.Description, entity.Skills);

            await _summaryRepository.UpdateAsync(summary);

            return entity;
        }
    }

}
