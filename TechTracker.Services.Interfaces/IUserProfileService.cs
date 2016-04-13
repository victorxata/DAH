using System.Collections.Generic;
using System.Threading.Tasks;
using TechTracker.Domain.Data.Identity;

namespace TechTracker.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<ApplicationUser> GetUser(string userId);

        PreRegisteredUser GetPreRegisteredUserByToken(string token, out int status);

        Task<string> SavePreRegisteredUser(PreRegisteredUser user, bool sendEmail = true, bool inviteToTenant = false);

        Task<string> IsValid(PreRegisteredUser user);

        Task<UsernameValidityState> UsernameIsValidAsync(string username);

        void UpdateRoles(string userId, List<string> roles);

        Task RegisterUserAsync(string id);

        Task<string> TryToSendEmailRecovery(string username);

        Task<RecoverPassword> GetRecoverPasswordByToken(string token);

        Task<int> CanChangePassword(string token);

        Task<string> GetUserId(string userName);

        void DeleteUser(ApplicationUser user);

        Task RegisterUserByTokenAsync(string preRegisterToken);

        Task<string> SendRecoveryEmail(string username, string callbackUrl);

        
    }
}
