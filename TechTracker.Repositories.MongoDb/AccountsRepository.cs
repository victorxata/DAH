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
    public class AccountsRepository : MongoRepository<Account>, IAccountsRepository
    {
        private readonly ISummaryRepository _summaryRepository;

        public AccountsRepository(ISummaryRepository summaryRepository)
        {
            _summaryRepository = summaryRepository;
        }

        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<Account> col)
        {
            var desc = Builders<Account>.IndexKeys.Ascending(t => t.Description);
            var unique = new CreateIndexOptions { Unique = true, Sparse = true};
            col.Indexes.CreateOneAsync(desc, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.Accounts;

        public override async Task<Account> AddAsync(Account entity)
        {
            var summary = await _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year)).FirstOrDefaultAsync()
                          ??
                          new Summary { Year = DateTime.UtcNow.Year };

            await base.AddAsync(entity);

            summary = summary.AddAccount(entity.Id, entity.Description);

            await _summaryRepository.UpdateAsync(summary);

            return entity;
        }

        public override async Task DeleteAsync(Account entity)
        {
            var summary =
                await
                    _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year))
                        .FirstOrDefaultAsync();
            if (summary != null)
            {
                summary = summary.RemoveAccount(entity.Id);
                await _summaryRepository.UpdateAsync(summary);
            }

            await base.DeleteAsync(entity);
        }

        public override async Task<Account> UpdateAsync(Account entity)
        {
            var summary =
                   await
                       _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year))
                           .FirstOrDefaultAsync();
            if (summary != null)
            {
                summary = summary.UpdateAccount(entity.Id, entity.Description);
                await _summaryRepository.UpdateAsync(summary);
            }
            await base.UpdateAsync(entity);

            return entity;
        }
    }
}
