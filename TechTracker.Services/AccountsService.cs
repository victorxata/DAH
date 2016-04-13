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
    public class AccountsService : IAccountsService
    {
        private readonly IAccountsRepository _skillsRepository;

        private readonly ILog Log;

        public AccountsService(IAccountsRepository skillsRepository, ILog log)
        {
            _skillsRepository = skillsRepository;
            Log = log;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            return await _skillsRepository.Collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task<Account> GetAccountByIdAsync(string skillId)
        {
            return await _skillsRepository.GetByIdAsync(skillId);
        }

        public async Task<Account> AddAccountAsync(Account skill)
        {
            return await _skillsRepository.AddAsync(skill);
        }

        public async Task<Account> UpdateAccountAsync(Account skill)
        {
            return await _skillsRepository.UpdateAsync(skill);
        }

        public async Task DeleteAccountAsync(string skillId)
        {
            await _skillsRepository.DeleteAsync(skillId);
        }
    }
}
