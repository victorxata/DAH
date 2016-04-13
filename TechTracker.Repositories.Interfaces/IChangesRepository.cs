using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Core.MongoDb.Changes;

namespace TechTracker.Repositories.Interfaces
{
    public interface IChangesRepository : IRepository<Change>
    {
    }
}
