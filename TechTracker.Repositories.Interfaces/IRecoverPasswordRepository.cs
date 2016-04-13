using TechTracker.Domain.Data.Core.MongoDb;
using TechTracker.Domain.Data.Identity;

namespace TechTracker.Repositories.Interfaces
{
    public interface IRecoverPasswordRepository : IRepository<RecoverPassword>
    {
    }
}
