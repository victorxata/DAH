using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Models.Business;

namespace TechTracker.Services.Interfaces
{
    public interface IAccountsService
    {
        Task<IEnumerable<Account>> GetAccountsAsync();
        
        Task<Account> GetAccountByIdAsync(string accountId);
        
        Task<Account> AddAccountAsync(Account account);
       
        Task<Account> UpdateAccountAsync(Account account);
        
        Task DeleteAccountAsync(string accountId);
    }
}
