using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using TechTracker.Domain.Data.Models.Business;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class OpportunitiesService : IOpportunitiesService
    {
        private readonly IOpportunitiesRepository _opportunitiesRepository;

        private readonly ISkillsService _skillsService;

        private readonly IAccountsService _accountsService;

        private readonly ISummaryService _summaryService;

        private readonly ILog Log;

        public OpportunitiesService(IOpportunitiesRepository opportunitiesRepository, ILog log, ISkillsService skillsService, ISummaryService summaryService, IAccountsService accountsService)
        {
            _opportunitiesRepository = opportunitiesRepository;
            Log = log;
            _skillsService = skillsService;
            _summaryService = summaryService;
            _accountsService = accountsService;
        }

        public async Task<IEnumerable<Opportunity>> GetOpportunityAsync()
        {
            return await _opportunitiesRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Opportunity> GetOpportunityByIdAsync(string skillId)
        {
            return await _opportunitiesRepository.GetByIdAsync(skillId);
        }

        public async Task<Opportunity> AddOpportunityAsync(Opportunity opp)
        {
            return await _opportunitiesRepository.AddAsync(opp);
        }

        public async Task<Opportunity> UpdateOpportunityAsync(Opportunity opp)
        {
            return await _opportunitiesRepository.UpdateAsync(opp);
        }

        public async Task DeleteOpportunityAsync(string skillId)
        {
            await _opportunitiesRepository.DeleteAsync(skillId);
        }

        public async Task<Opportunity> AddSkillAsync(string oppId, Skill skill)
        {
            var opp = await GetOpportunityByIdAsync(oppId);
            if (opp.Skills == null) opp.Skills = new List<Skill>();
            opp.Skills.Add(skill);

            var summary = await _summaryService.GetSummaryAsync();
            var account = await _accountsService.GetAccountByIdAsync(opp.AccountId);
            account.TargetPositions = account.TargetPositions + 1;
            await _accountsService.UpdateAccountAsync(account);

            summary = summary.AddAccount(account.Id, account.Description);
            summary = summary.AddSkill(skill.Id, skill.Description);
            summary = summary.IncreaseTarget(skill.Id, account.Id, 1);
            summary.Target = summary.Target + 1;
            await _summaryService.UpdateSummaryAsync(summary);
            await UpdateOpportunityAsync(opp);
            return opp;
        }

        public async Task<Opportunity> RemoveSkillAsync(string oppId, string skillId)
        {
            var opp = await GetOpportunityByIdAsync(oppId);

            var summary = await _summaryService.GetSummaryAsync();
            var account = await _accountsService.GetAccountByIdAsync(opp.AccountId);
            account.TargetPositions = account.TargetPositions - 1;
            await _accountsService.UpdateAccountAsync(account);

            summary = summary.AddAccount(account.Id, account.Description);
            summary = summary.IncreaseTarget(skillId, account.Id, -1);
            summary.Target = summary.Target + 1;
            await _summaryService.UpdateSummaryAsync(summary);
            await UpdateOpportunityAsync(opp);
            return opp;
        }
    }
}
