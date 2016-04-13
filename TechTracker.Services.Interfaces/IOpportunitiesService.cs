using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.Business;

namespace TechTracker.Services.Interfaces
{
    public interface IOpportunitiesService
    {
        Task<IEnumerable<Opportunity>> GetOpportunityAsync();
        
        Task<Opportunity> GetOpportunityByIdAsync(string id);
        
        Task<Opportunity> AddOpportunityAsync(Opportunity skill);
       
        Task<Opportunity> UpdateOpportunityAsync(Opportunity skill);
        
        Task DeleteOpportunityAsync(string id);

        Task<Opportunity> AddSkillAsync(string oppId, Skill skill);

        Task<Opportunity> RemoveSkillAsync(string oppId, string skillId);
    }
}
