using MongoDB.Driver;
using TechTracker.Common.Utils;
using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Models.Email;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public class EmailTemplatesRepository : MongoRepository<EmailTemplate>, IEmailTemplatesRepository
    {
        protected override bool IsGlobal => true;

        protected override void EnsureIndexes(string tenantId, IMongoCollection<EmailTemplate> col)
        {
            var cName = Builders<EmailTemplate>.IndexKeys.Ascending(t => t.Name);
            var unique = new CreateIndexOptions { Unique = true };
            col.Indexes.CreateOneAsync(cName, unique);
        }

        protected override bool TrackChanges => false;

        protected override TenantDatabase DbType => TenantDatabase.TenantMongoDb;

        protected override string CollectionName => Constants.MongoDbSettings.CollectionNames.EmailTemplates;
    }
}
