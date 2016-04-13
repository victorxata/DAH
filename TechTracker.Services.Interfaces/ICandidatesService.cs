using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.Business;

namespace TechTracker.Services.Interfaces
{
    public interface ICandidatesService
    {
        Task<IEnumerable<Candidate>> GetCandidateAsync();
        
        Task<Candidate> GetCandidateByIdAsync(string skillId);
        
        Task<Candidate> AddCandidateAsync(Candidate skill);
       
        Task<Candidate> UpdateCandidateAsync(Candidate skill);
        
        Task DeleteCandidateAsync(string skillId);
    }
}
