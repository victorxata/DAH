using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Domain.Data.Models.RBAC;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class SkillsRepository : MongoRepository<Skill>, ISkillsRepository
    {
        private readonly ISummaryRepository _summaryRepository;

        public SkillsRepository(ISummaryRepository summaryRepository)
        {
            _summaryRepository = summaryRepository;
        }

        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<Skill> col)
        {
            var desc = Builders<Skill>.IndexKeys.Ascending(t => t.Description);
            var unique = new CreateIndexOptions { Unique = true, Sparse = true};
            col.Indexes.CreateOneAsync(desc, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.Skills;

        public override async Task<Skill> AddAsync(Skill entity)
        {
            var summary = await _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year)).FirstOrDefaultAsync() 
                          ?? 
                          new Summary {Year = DateTime.UtcNow.Year};

            await base.AddAsync(entity);

            summary = summary.AddSkill(entity.Id, entity.Description);

            await _summaryRepository.UpdateAsync(summary);

            return entity;
        }

        public override async Task DeleteAsync(Skill entity)
        {
            var summary =
                await
                    _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year))
                        .FirstOrDefaultAsync();
            if (summary != null)
            {
                summary = summary.RemoveSkill(entity.Id);
                await _summaryRepository.UpdateAsync(summary);
            }

            await base.DeleteAsync(entity);
        }

        public override async Task<Skill> UpdateAsync(Skill entity)
        {
            var summary =
                   await
                       _summaryRepository.Collection.Find(new BsonDocument("Year", DateTime.UtcNow.Year))
                           .FirstOrDefaultAsync();
            if (summary != null)
            {
                summary = summary.UpdateSkill(entity.Id, entity.Description);
                await _summaryRepository.UpdateAsync(summary);
            }
            await base.UpdateAsync(entity);

            return entity;
        }
    }
}
