using System.Configuration;
using System.Web;
using MongoDB.Driver;
using Microsoft.AspNet.Identity.Owin;
using TechTracker.Domain.Data.Identity.Aspnet;

namespace TechTracker.Domain.Data.Identity
{
    public static class Managers
    {
        //private static ApplicationUserManager _userManager;

        private static ApplicationRoleManager _roleManager;

        public static ApplicationUserManager Users
        {
            get {
                return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public static ApplicationRoleManager Roles
        {
            get {
                return _roleManager ??
                       (_roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>());
            }
        }

        public static IMongoCollection<ApplicationUser> UsersCollection
        {
            get
            {
                var client = new MongoClient(ConfigurationManager.AppSettings[Common.Utils.Constants.MongoDbAppSettings.GlobalDbMongoDbServer] +
                "/" + ConfigurationManager.AppSettings[Common.Utils.Constants.MongoDbAppSettings.GlobalDbMongoDatabaseName]);
                var database = client.GetDatabase(ConfigurationManager.AppSettings[Common.Utils.Constants.MongoDbAppSettings.GlobalDbMongoDatabaseName]);
                var users = database.GetCollection<ApplicationUser>(Common.Utils.Constants.MongoDbSettings.CollectionNames.Users);

                return users;
            }
        } 
    }
}
