using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class SkillsService : ISkillsService
    {
        private readonly ISkillsRepository _skillsRepository;

        private readonly ILog Log;

        public SkillsService(ISkillsRepository skillsRepository, ILog log)
        {
            _skillsRepository = skillsRepository;
            Log = log;
        }

        public async Task<IEnumerable<Skill>> GetSkillAsync()
        {
            return await _skillsRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Skill> GetSkillByIdAsync(string skillId)
        {
            return await _skillsRepository.GetByIdAsync(skillId);
        }

        public async Task<Skill> AddSkillAsync(Skill skill)
        {
            return await _skillsRepository.AddAsync(skill);
        }

        public async Task<Skill> UpdateSkillAsync(Skill skill)
        {
            return await _skillsRepository.UpdateAsync(skill);
        }

        public async Task DeleteSkillAsync(string skillId)
        {
            await _skillsRepository.DeleteAsync(skillId);
        }
    }
}
