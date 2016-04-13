using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using TechTracker.Api.Common.ExternalLoginData;
using TechTracker.Api.Common.Providers;
using TechTracker.Api.Results;
using TechTracker.Domain.Data.Identity;
using TechTracker.Domain.Data.Identity.Aspnet;
using TechTracker.Services.Interfaces;
using ApplicationUserManager = TechTracker.Domain.Data.Identity.Aspnet.ApplicationUserManager;

namespace TechTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("api/UserProfile")]
    public class UserProfileController : BaseController
    {
        private readonly IUserProfileService _userProfileServices;

        private ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; set; }

        private readonly ApplicationUserManager _userManager = Managers.Users;
        private readonly ApplicationRoleManager _roleManager = Managers.Roles;

        private const string LocalLoginProvider = "Local";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userProfileServices"></param>
        public UserProfileController(ILog logger, IUserProfileService userProfileServices) : base(logger)
        {
            _userProfileServices = userProfileServices;
            AccessTokenFormat = Startup.OAuthOptions.AccessTokenFormat;
        }

        // GET api/Account/UserInfo
        /// <summary>
        /// Gets the information of the current user (if logged in)
        /// </summary>
        /// <returns>The username and the provider</returns>
        [HttpGet]
        [Route("UserInfo")]
        public IHttpActionResult GetUserInfo()
        {
            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return NotFound();
            }
            var result = new UserInfoModel
            {
                UserName = User.Identity.GetUserName(),
                HasRegistered = false,
                LoginProvider = externalLogin.LoginProvider
            };
            return Ok(result);
        }

        // GET api/Account/ExternalLogin
        /// <summary>
        /// Login with an external provider
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="error"></param>
        /// <returns>HTTP Code with result of the operation. 200 if successful</returns>
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            // There can only be one social provider (facebook, twitter, etc).
            // However, Active Directory is such that there can be many servers,
            // so the "Provider" value that we get from ASP Idenity won't be Federation,
            // it will be the signature of the application registered on azure.
            if (externalLogin.LoginProvider != provider && !(provider == "Federation" && externalLogin.LoginProvider.StartsWith("https://sts.windows.net")))
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var userLoginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);

            var user = _userManager.Find(userLoginInfo);

            var hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var oAuthIdentity = await _userManager.CreateIdentityAsync(user, OAuthDefaults.AuthenticationType);
                var cookieIdentity = await _userManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);
                var properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                var claims = externalLogin.GetClaims();
                var identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok("Done");
        }

        // POST api/Account/Logout
        /// <summary>
        /// Logs out of the application (removes the cookie or valid token)
        /// </summary>
        /// <returns></returns>
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok("Logged out.");
        }

        // POST api/Account/ChangePassword
        /// <summary>
        /// Changes the password of an user
        /// </summary>
        /// <param name="model">An object with the current password and the new and its confirmation</param>
        /// <returns>HTTP Code with result of the operation. 200 if successful</returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel model)
        {
            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            var errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return errorResult;
            }
            return Ok("Password changed");
        }

        // POST api/Account/SetPassword
        /// <summary>
        /// Sets the password of an user
        /// </summary>
        /// <param name="model">An object with the new password and its confirmation</param>
        /// <returns>HTTP Code with result of the operation. 200 if successful</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword([FromBody]SetPasswordModel model)
        {
            var token = model.Token;
            var changePassword = await _userProfileServices.CanChangePassword(token);

            if (changePassword == 200)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    return NotFound();
                }

                await _userManager.RemovePasswordAsync(user.Id);
                var result = await _userManager.AddPasswordAsync(user.Id, model.Password);

                var errorResult = GetErrorResult(result);
                if (errorResult != null)
                {
                    return errorResult;
                }
                return Ok("Password set");
            }
            switch (changePassword)
            {
                case 404:
                    return NotFound();
                default:
                    return BadRequest();
            }
        }
       
        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        /// <summary>
        /// Returns all the information needed to manage the user info
        /// </summary>
        /// <param name="returnUrl">The url where to go after retrieving the data</param>
        /// <param name="generateState"></param>
        /// <returns>A list with all the Login Information about the user, like the logins (internal or external) associated to the current user</returns>
        [Route("ManageInfo")]
        public async Task<IHttpActionResult> GetManageInfo(string returnUrl, bool generateState = false)
        {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return NotFound();
            }

            var logins = user.Logins.Select(linkedAccount => new UserLoginInfoModel
            {
                LoginProvider = linkedAccount.LoginProvider,
                ProviderKey = linkedAccount.ProviderKey
            }).ToList();

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            var result = new ManageInfoModel
            {
                LocalLoginProvider = LocalLoginProvider,
                UserName = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
            return Ok(result);
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        /// <summary>
        /// Retrieve the list of External Logins availables
        /// </summary>
        /// <param name="returnUrl">The Url to return after login</param>
        /// <param name="generateState">By default false. If true, it generates a random OAuth State to be used in the returned objects</param>
        /// <returns>A list of ExternalLoginView with the Provider, Response_Type, Client_Id, Redirect_Uri and State fields</returns>
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            var descriptions = Authentication.GetExternalAuthenticationTypes();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            var loginVMs = descriptions.Select(description => new ExternalLoginModel
            {
                Name = description.Caption,
                Url = Url.Link("ExternalLogin", new
                {
                    provider = description.AuthenticationType,
                    response_type = "token",
                    client_id = Startup.PublicClientId,
                    redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                    state
                }),
                State = state
            }).ToList();

            return loginVMs;
        }

        // POST api/Account/AddExternalLogin
        /// <summary>
        /// Adds a new external login for a given user
        /// </summary>
        /// <param name="model">Provides a token with the external login</param>
        /// <returns>HTTP Code with result of the operation. 200 if successful</returns>
        [HttpPost]
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginModel model)
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            var externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            var errorResult = GetErrorResult(result);

            return errorResult ?? Ok(externalData);
        }

        //POST api/Account/RemoveLogin
        /// <summary>
        /// Removes a login provider from a user
        ///</summary>
        ///<param name="model"></param>
        ///<returns>HTTP Code with result of the operation. 200 if successful</returns>
        [HttpPost]
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginModel model)
        {
            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await _userManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await _userManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            var errorResult = GetErrorResult(result);

            return errorResult ?? Ok("External Login removed");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("externalUserExists")]
        public IHttpActionResult CheckIfExternalUserExists()
        {
            ApplicationUser result;

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if ((externalLogin == null) && (User.Identity.IsAuthenticated))
            {
                result = _userManager.FindById(User.Identity.GetUserId());
            }
            else
            {
                if (externalLogin != null)
                {
                    var userLoginInfo = new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey);

                    var user = _userManager.Find(userLoginInfo);

                    return Ok(user);
                }
                return NotFound();
            }

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("rememberMe/{username}")]
        public async Task<IHttpActionResult> RememberMe(string username)
        {
            var token = await _userProfileServices.TryToSendEmailRecovery(username);
            return Ok(token);
        }

        // POST api/Account/Preregistration
        /// <summary>
        /// Register an user contained in the RegisterBindingModel provided
        /// </summary>
        /// <param name="model">An object of the kind RegisterBindingModel</param>
        /// <returns>An HTTP Code:
        ///             200: Registered if the user has been registered successfully 
        ///             or Http Error Code if not registered</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("preregistration")]
        public async Task<IHttpActionResult> Preregistration([FromBody]PreRegisteredUser model)
        {
            var validation = await _userProfileServices.IsValid(model);
            string token;
            if (!string.IsNullOrEmpty(validation))
            {
                return BadRequest(validation);
            }
            try
            {
                token = await _userProfileServices.SavePreRegisteredUser(model);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/Account/Registration?Token=asd;lfkjadf;lkjadf;ljk
        /// <summary>
        /// Retrieve preregistered user based on the token
        /// </summary>
        /// <param name="token">The token to retrieve the pre registration user data</param>
        /// <returns>A list of ExternalLoginView with the Provider, Response_Type, Client_Id, Redirect_Uri and State fields</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("registration")]
        public IHttpActionResult GetRegistration([FromUri]string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            int status;
            var prereg = _userProfileServices.GetPreRegisteredUserByToken(token, out status);

            switch (status)
            {
                case 410:
                    throw new HttpResponseException(HttpStatusCode.Gone);
                case 404:
                    return NotFound();
                case 400:
                    return BadRequest();
                default:
                    if (prereg == null)
                    {
                        return NotFound();
                    }
                    var result = new RegisterModel
                    {
                        UserName = prereg.UserName,
                        Email = prereg.EmailAddress,
                        PreRegisteredUser = prereg,
                        PreRegisteredUserId = prereg.Id
                    };
                    return Ok(result);

            }
        }

        //POST api/Account/Register
        ///<summary>
        ///Register an user contained in the RegisterBindingModel provided
        ///</summary>
        ///<param name="model">An object of the kind RegisterBindingModel</param>
        ///<returns>An HTTP Code:
        ///            200: Registered if the user has been registered successfully 
        ///            or Http Error Code if not registered</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("registration")]
        public async Task<IHttpActionResult> UpdateRegistrationWithPreregistion([FromBody] RegisterModel model)
        {
            if (String.IsNullOrEmpty(model.PreRegisteredUserId))
            {
                return BadRequest("Cannot create a user without PreRegistration");
            }

        var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                RealName = model.RealName,
                ImageUrl = model.ImageUrl
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            var errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            await _userProfileServices.RegisterUserAsync(model.PreRegisteredUserId);

            return Ok("Registered.");
        }

        /// <summary>
        /// Function to check if the user name is valid
        /// </summary>
        /// <param name="name">User name to check</param>
        /// <returns>Ok if the username is valid</returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("checkname")]
        public async Task<IHttpActionResult> CheckUsername([FromUri]string name)
        {
            var validState = await _userProfileServices.UsernameIsValidAsync(name);
            return Ok(validState);
        }

        /// <summary>
        /// This method is used when a user registers from an external provider, the main difference being the required authorization, and removal of the RegisterUser method.
        /// </summary>
        /// <param name="model">The model to map to the ApplicationUser</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("externalregistration")]
        public async Task<IHttpActionResult> UpdateRegistrationWithExternalProvider([FromBody] RegisterModel model)
        {
            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            var username = model.UserName;
            var email = model.Email;
            var user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                RealName = model.RealName,
                ImageUrl = model.ImageUrl
            };
            user.Logins.Add(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            var result = await _userManager.CreateAsync(user, "2c517F10-C697-4763-A814-83DFB9FEFE0A"); // TODO: Validate this password when getting new token
            
            var errorResult = GetErrorResult(result);
            if (errorResult != null)
            {
                return errorResult;
            }
            return Ok("Registered");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="preRegisterToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("preregister/{preRegisterToken}/deactivate")]
        public async Task<IHttpActionResult> DeactivateRegistration([FromUri]string preRegisterToken)
        {
            await _userProfileServices.RegisterUserByTokenAsync(preRegisterToken);
            return Ok();
        }


        // POST: api/Account/RegisterExtended
        /// <summary>
        /// Register an user contained in the RegisterViewModel provided
        /// </summary>
        /// <param name="model">An object of the kind RegisterBindingModel</param>
        /// <returns>An HTTP Code:
        ///             200: Registered if the user has been registered successfully 
        ///             or Http Error Code if not registered</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterExtended")]
        public async Task<IHttpActionResult> Register(RegisterModel model)
        {
            var user = new ApplicationUser();
            user.ApplyChanges(model);

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("Registered");
            }

            var errors = result.Errors.Aggregate("", (current, error) => $"{current}\r\n{error}");

            return BadRequest(errors);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("user/{userId}")]
        public IHttpActionResult DeleteUser(string userId)
        {
            var user = _userManager.FindById(userId);
            _userManager.Delete(user);

            return Ok("Deleted.");
        }

        //
        // GET: /Account/EditProfile
        /// <summary>
        /// Gets the profile data of the current user. If the current user is not found,returns a 204 (NotFound)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("profile")]
        public async Task<IHttpActionResult> GetProfile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.GetUserName());

            if (user == null)
            {
                return NotFound();
            }

            var result = new RegisterModel();
            result.RegisterModelFactory(user);

            return Ok(result);
        }

        //
        // POST: /Account/EditProfile
        /// <summary>
        /// Updates the profile sent in the body
        /// </summary>
        /// <param name="model">Content of the profile to update</param>
        /// <returns>IHttpActionResult</returns>
        [HttpPost]
        [Route("profile")]
        public async Task<IHttpActionResult> EditProfile([FromBody]RegisterModel model)
        {
            var oldUser = _userManager.FindById(User.Identity.GetUserId());
            if (oldUser == null)
                return NotFound();

            oldUser.ApplyChanges(model);

            var result = await _userManager.UpdateAsync(oldUser);

            var errorResultR = GetErrorResultR(result);
            if (errorResultR != null)
            {
                return errorResultR;
            }

            if (result != null)
            {
                return Ok();
            }
            return BadRequest();
        }


        /// <summary>
        /// Gets the profile of a user
        /// </summary>
        /// <param name="username">The user name to get his profile</param>
        /// <returns></returns>
        [HttpGet]
        [Route("profile/{username}")]
        public async Task<IHttpActionResult> GetProfile(string username)
        {
            if (User.Identity.GetUserName() != username)
                return Unauthorized();

            var user = await Task.Run(() => _userManager.FindByName(username));

            var result = new RegisterModel();
            result.RegisterModelFactory(user);

            return Ok(result);

        }

        /// <summary>
        /// Updates a profile
        /// </summary>
        /// <param name="id">The username's id to update his profile</param>
        /// <param name="model">The object with the user profile</param>
        /// <returns></returns>
        [HttpPut]
        [Route("profile/{id}")]
        public async Task<IHttpActionResult> PutProfile(string id, [FromBody] RegisterModel model)
        {
            if(User.Identity.GetUserId() != id)
            {
                return Unauthorized();
            }

            var oldUser = _userManager.FindById(id);
            if (oldUser == null)
                return NotFound();
            
            oldUser.ApplyChanges(model);

            var result = await _userManager.UpdateAsync(oldUser);

            var errorResultR = GetErrorResultR(result);
            if (errorResultR != null)
            {
                return errorResultR;
            }

            if (result != null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Created);
                response.Headers.Location = new Uri(Request.RequestUri, id);

                IHttpActionResult resultado = ResponseMessage(response);
                return resultado; 
            }

            return BadRequest(); 
        }

        /// <summary>
        /// Adds a user to a role
        /// </summary>
        /// <param name="userId">The Id of the User</param>
        /// <param name="role">The role</param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Route("profile/{userId}/AddToRole/{role}")]
        public async Task<IHttpActionResult> AddUserToRole(string userId, string role)
        {
            await _userManager.AddToRoleAsync(userId, role);
            return Ok();
        }

       

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (result.Succeeded) return null;

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error);
                }
            }

            return BadRequest(ModelState);
        }

        private IHttpActionResult GetErrorResultR(IdentityResult result)
        {
            if (result == null)
            {
                return new InternalServerErrorResult(Request);
            }

            if (result.Succeeded) return null;

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Error", error);
                }
            }

            return new BadRequestResult(new HttpRequestMessage(HttpMethod.Put, ModelState.ToString()));
        }

        private static class RandomOAuthStateGenerator
        {
            private static readonly RandomNumberGenerator Random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException(@"strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                var strengthInBytes = strengthInBits / bitsPerByte;

                var data = new byte[strengthInBytes];
                Random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
