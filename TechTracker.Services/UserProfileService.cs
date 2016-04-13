using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using TechTracker.Domain.Data.Identity;
using TechTracker.Domain.Data.Identity.Aspnet;
using TechTracker.Repositories.Interfaces;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IPreRegistrationRepository _preRegistrationRepository;
        private readonly IRecoverPasswordRepository _recoverPasswordRepository;
        private readonly IEmailService _emailServices;

        private readonly ILog Log;

        public UserProfileService(
                               IPreRegistrationRepository preRegistrationRepository
                               , IEmailService emailServices
                               , IRecoverPasswordRepository recoverPasswordRepository, ILog log)
        {
            _preRegistrationRepository = preRegistrationRepository;
            _emailServices = emailServices;
            _recoverPasswordRepository = recoverPasswordRepository;
            Log = log;
        }

        private ApplicationUserManager UserManager => Managers.Users;

        public async Task<ApplicationUser> GetUser(string userId)
        {
            var result = await UserManager.FindByIdAsync(userId);
            return result;
        }
        

        public PreRegisteredUser GetPreRegisteredUserByToken(string token, out int status)
        {
            var user =
                    _preRegistrationRepository.Collection.Find(x => x.Token == token).FirstOrDefaultAsync().Result;

                if (user == null)
                {
                    status = 404; // not found
                    return null;
                }
                if (user.Activated)
                {
                    status = 410; // Gone
                    return null;
                }
                if (user.ValidUntil <= DateTime.UtcNow)
                {
                    status = 400; // bad request
                    return null;
                }
                status = 200;   // found ok
                return user;
            
        }

        public async Task<string> SavePreRegisteredUser(PreRegisteredUser user, bool sendEmail = true, bool inviteToTenant = false)
        {

            user.ValidUntil =
                DateTime.UtcNow.AddDays(
                    Convert.ToInt32(ConfigurationManager.AppSettings["ValidPreregistrationDays"] ?? "5"));
            user.Token = Guid.NewGuid().ToString().Replace("-", "");
            user.Activated = false;
            user = await _preRegistrationRepository.AddAsync(user);
            if (sendEmail && !inviteToTenant)
            {
                await
                    _emailServices.SendEmail(user.EmailAddress, "TechTrackerX PreRegistration", "Preregistration",
                    new Dictionary<string, string>
                    {
                        {
                            "TOKEN",
                            $"{ConfigurationManager.AppSettings["WebRoute"]}/register/{user.Token}"
                        },
                        {
                            "NAME", 
                            user.UserName
                        }
                    },
                    "EN-US");
            }
            else if (sendEmail)
            {

                await
                    _emailServices.SendEmail(user.EmailAddress, "TechTrackerX Invitation", "TenantInvite",
                    new Dictionary<string, string>
                    {
                        {
                            "LINK",
                            $"{ConfigurationManager.AppSettings["WebRoute"]}/account/tenantinvite/{user.Token}"
                        }
                    },
                    "EN-US");
            }
            return user.Token;
        }

        public async Task<string> IsValid(PreRegisteredUser user)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(user.UserName))
                    return "User name is mandatory";
                _preRegistrationRepository.DeleteAsync(x => x.UserName == user.UserName).Wait();
                _preRegistrationRepository.DeleteAsync(x => x.EmailAddress == user.EmailAddress).Wait();

                if (string.IsNullOrEmpty(user.EmailAddress))
                    return "Email address is mandatory";

                if (HttpContext.Current == null)
                    return null;

                var userExists = UserManager.FindByNameAsync(user.UserName).Result;

                return userExists != null ? "The user name selected is already taken" : null;
            });
            return string.Empty;
        }

        public void DeleteUser(ApplicationUser user)
        {
            UserManager.Delete(user);

        }

        public async Task<UsernameValidityState> UsernameIsValidAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return UsernameValidityState.Empty;

            var userExists = await UserManager.FindByNameAsync(username);
            return userExists != null ? UsernameValidityState.Taken : UsernameValidityState.Ok;
        }

        public void UpdateRoles(string userId, List<string> roles)
        {
            //var intRoles = _appRoleRepository.Get().Where(r => roles.Contains(r.Name));
            //var user = _usersRepository.Get().First(x => x.CorrelationId == userId);
            //foreach (var intRole in intRoles)
            //{
            //    user.Roles.Add(new IntUserRole() { RoleId = intRole.CorrelationId, UserId = user.CorrelationId });
            //}
            //_usersRepository.Update(user.CorrelationId, user);
        }

        public async Task RegisterUserAsync(string id)
        {
            if (id == null)
            {
                var exception = new ArgumentNullException(@"An id for a pre-registered use must be supplied.");
                throw exception;
            }

            var preuser = await _preRegistrationRepository.GetByIdAsync(id); //Collection.Find(x => x.Token == id).FirstOrDefaultAsync());

            if (preuser == null) return;

            preuser.Activated = true;
            
            await _preRegistrationRepository.UpdateAsync(preuser);
        }

        public async Task RegisterUserByTokenAsync(string id)
        {
            if (id == null)
            {
                var exception = new ArgumentNullException("An id for a pre-registered use must be supplied.");
                throw exception;
            }

            var preuser = await _preRegistrationRepository.Collection.Find(x => x.Token == id).FirstOrDefaultAsync();

            if (preuser == null) return;

            preuser.Activated = true;
            await _preRegistrationRepository.UpdateAsync(preuser);
        }

        public async Task<string> SendRecoveryEmail(string username, string callbackUrl)
        {
            var userExists = UserManager.FindByName(username);

            if (userExists == null)
                return await Task.Run(()=> string.Empty);

            var valid = ConfigurationManager.AppSettings["ValidRecoverPasswordTokenDays"] ?? "5";
            var validUntil = DateTime.UtcNow.AddDays(Convert.ToInt32(valid));
            var token = Guid.NewGuid().ToString().Replace("-", "");
            var recoverPassword = new RecoverPassword
            {
                UserName = username,
                ValidUntil = validUntil,
                Token = token,
                Activated = false
            };
            await _recoverPasswordRepository.AddAsync(recoverPassword);

            var templateDictionary = new Dictionary<string, string>
            {
                {
                    "TOKEN",
                    $"{callbackUrl}?token={token}"
                },
                {
                    "NAME",
                    username
                }
            };
            const string lang = "en-US";

            await _emailServices.SendEmail(userExists.Email, "TechTracker Recover Password", "RecoverPassword", templateDictionary, lang);
            return token;
        }

        public async Task<string> TryToSendEmailRecovery(string username)
        {
            var userExists = UserManager.FindByName(username);

            if (userExists == null)
                return String.Empty;

            var valid = ConfigurationManager.AppSettings["ValidPreregistrationDays"] ?? "5";
            var validUntil = DateTime.UtcNow.AddDays(Convert.ToInt32(valid));
            var token = Guid.NewGuid().ToString().Replace("-", "");
            var recoverPassword = new RecoverPassword
            {
                UserName = username,
                ValidUntil = validUntil,
                Token = token,
                Activated = false
            };
            await _recoverPasswordRepository.AddAsync(recoverPassword);

            var templateDictionary = new Dictionary<string, string>
            {
                {
                    "TOKEN",
                    $"{ConfigurationManager.AppSettings["WebRoute"]}/login/recoverpassword?token={token}"
                },
                {
                    "NAME", 
                    username
                }
            };
            const string lang = "en-US";

            await _emailServices.SendEmail(userExists.Email, "TechTrackerX Recover Password", "RecoverPassword", templateDictionary, lang);
            return token;
        }

        public async Task<RecoverPassword> GetRecoverPasswordByToken(string token)
        {
            return await _recoverPasswordRepository.Collection.Find(x => x.Token == token).FirstOrDefaultAsync();
        }

        public async Task<int> CanChangePassword(string token)
        {
            var recoverPassword = await _recoverPasswordRepository.Collection.Find(x => x.Token == token).FirstOrDefaultAsync();

                if (recoverPassword == null)
                {
                    return 404; // not found
                }
                if (recoverPassword.ValidUntil <= DateTime.UtcNow)
                {
                    return 400; // bad request
                }
                return 200;   // found ok
        }

        public async Task<string> GetUserId(string userName)
        {
            var result = await UserManager.FindByNameAsync(userName);

            return result?.Id;
        }

        //public Task<ApplicationUser> UpdateUserProfile(UserProfile model)
        //{
        //    var user = new ApplicationUser
        //    {
        //        CorrelationId = model.UserId,
        //        Birthday = model.Birthday,
        //        Bio = model.Bio,
        //        Active = true,
        //        EmailAddress = model.EmailAddress,
        //        InternationalPhone = model.InternationalPhone,
        //        JobTitle = model.JobTitle,
        //        PgpKey = model.PgpKey,
        //        RealName = model.RealName,
        //        WebSite = model.WebSite,
        //        ImageUrl = model.ImageUrl,
        //        InternationalAddress = model.InternationalAddress
        //    };

        //    var result = Task.Run(() => _usersRepository.Update(user.CorrelationId, user));
        //    return result;
        //}

        //public AccountSettingsModel GetAccountSettings(string username)
        //{
        //    var user = _usersRepository.Get().FirstOrDefault(x => x.UserName == username);


        //    if (user == null)
        //        return null;

        //    _logger.LogInfo("AccountServices::GetAccountSettings", "User exists. Sending to the edit profile action the user data", new Dictionary<string, object> { { "user", user } });
        //    var model = new AccountSettingsModel
        //    {
        //        UserId = user.CorrelationId,
        //        CountryId = user.Country,
        //        LanguageId = user.Language,
        //        RealName = user.RealName,
        //        ImageUrl = user.ImageUrl,
        //        PgpKey = user.PgpKey
        //    };

        //    return model;

        //}

        //public Task<ApplicationUser> UpdateAccountSettings(AccountSettingsModel model)
        //{
        //    var user = new ApplicationUser
        //    {
        //        CorrelationId = model.UserId,
        //        Active = true,
        //        RealName = model.RealName,
        //        Language = model.LanguageId,
        //        Country = model.CountryId,
        //        PgpKey = model.PgpKey
        //    };

        //    var result = Task.Run(() => _usersRepository.Update(user.CorrelationId, user));

        //    return result;

        //}

        //public IEnumerable<UserProfile> GetUserProfiles()
        //{
        //    var roles = _appRoleRepository.Get().ToList();
        //    return _usersRepository.Get().Include(u => u.Roles).ToList().Select(user => CreateUserDto(user, roles));
        //}

        //public IEnumerable<UserProfile> SearchProfilesByRealName(string name)
        //{
        //    var roles = _appRoleRepository.Get().ToList();
        //    return _usersRepository.Get().Include(u => u.Roles).Where(x => x.RealName.StartsWith(name)).ToList().Select(user => CreateUserDto(user, roles));
        //}

        //public UserProfile GetUserProfileByProviderKey(string providerKey)
        //{
        //    var login = _usersRepository.Logins()
        //        .FirstOrDefault(x => x.ProviderKey == providerKey);
        //    if (login == null)
        //        return null;
        //    var user = _usersRepository.Get()
        //        .FirstOrDefault(x => x.CorrelationId == login.UserId);

        //    _logger.LogInfo("Web AccountController::GetUserProfileByProviderKey GET", "User exists. Sending to the edit profile action the user data", new Dictionary<string, object> { { "user", user } });
        //    return CreateUserDto(user);
        //}

        //public UserProfile GetUserProfileByUserId(int userId)
        //{
        //    var user = _usersRepository.Get().FirstOrDefault(x => x.CorrelationId == userId);

        //    _logger.LogInfo("Web AccountController::GetUserProfileByProviderKey GET", "User exists. Sending to the edit profile action the user data", new Dictionary<string, object> { { "user", user } });
        //    return CreateUserDto(user);
        //}

        //public UserProfile GetUserProfile(string username)
        //{
        //    var user = _usersRepository.Get()
        //        .FirstOrDefault(x => x.UserName == username);

        //    if (user == null)
        //        return null;

        //    _logger.LogInfo("Web AccountController::EditProfile GET", "User exists. Sending to the edit profile action the user data", new Dictionary<string, object> { { "user", user } });
        //    return CreateUserDto(user);
        //}

        //private UserProfile CreateUserDto(ApplicationUser user, List<IntRole> roles = null)
        //{
        //    if (roles == null)
        //    {
        //        roles = _appRoleRepository.Get().ToList();
        //    }

        //    var model = new UserProfile
        //    {
        //        UserId = user.CorrelationId,
        //        Bio = user.Bio,
        //        Birthday = user.Birthday,
        //        EmailAddress = user.EmailAddress,
        //        InternationalAddress = user.InternationalAddress,
        //        InternationalPhone = user.InternationalPhone,
        //        JobTitle = user.JobTitle,
        //        ImageUrl = user.ImageUrl,
        //        PgpKey = user.PgpKey,
        //        RealName = user.RealName,
        //        WebSite = user.WebSite
        //    };
        //    return model;
        //}
    }
}

