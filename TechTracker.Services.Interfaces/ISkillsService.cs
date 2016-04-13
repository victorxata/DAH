using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.Business;

namespace TechTracker.Services.Interfaces
{
    public interface ISkillsService
    {
        Task<IEnumerable<Skill>> GetSkillAsync();
        
        Task<Skill> GetSkillByIdAsync(string skillId);
        
        Task<Skill> AddSkillAsync(Skill skill);
       
        Task<Skill> UpdateSkillAsync(Skill skill);
        
        Task DeleteSkillAsync(string skillId);
    }
}
