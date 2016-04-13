using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.Business;

namespace TechTracker.Services.Interfaces
{
    public interface ISummaryService
    {
        Task<Summary> GetSummaryAsync();
        Task UpdateSummaryAsync(Summary summary);
    }
}
