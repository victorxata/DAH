using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using TechTracker.Api.Common.Middleware;
using TechTracker.Api.Common.Providers;
using TechTracker.Domain.Data.Identity;
using TechTracker.Domain.Data.Identity.Aspnet;

namespace TechTracker.Api
{
    public partial class Startup
    {

        /// <summary>
        /// 
        /// </summary>
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static string PublicClientId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static Func<UserManager<ApplicationUser>> UserManagerFactory { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        public virtual void ConfigureAuth(IAppBuilder app)
        {
            PublicClientId = "self";

            var appcontext = ApplicationIdentityContext.Create();
            UserManagerFactory =
                () => new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(appcontext.Users));
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                AuthorizeEndpointPath = new PathString("/api/UserProfile/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                AllowInsecureHttp = true,
                ApplicationCanDisplayErrors = true
            };

            app.CreatePerOwinContext(ApplicationIdentityContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/home/index"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity =
                        SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        TimeSpan.FromMinutes(30),
                        (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            app.UseOAuthBearerTokens(OAuthOptions);
            

            app.UseRbacPermissions();

        }


        private static string GetKeyValueFromConfig(string configKeyName)
        {
            var value = ConfigurationManager.AppSettings[configKeyName];

            if (value == null)
            {
                throw new ConfigurationErrorsException("Cannot find the App.config or Web.config key for " + configKeyName);
            }

            return value;
        }
    }
}
