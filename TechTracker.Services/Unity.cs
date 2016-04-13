using Microsoft.Practices.Unity;
using TechTracker.Services.Interfaces;

namespace TechTracker.Services
{
    public static class Unity
    {
        public static void Register(IUnityContainer container)
        {
           
            container.RegisterType<IEmailService, EmailService>(new PerResolveLifetimeManager());

            // Users
            container.RegisterType<IUserProfileService, UserProfileService>(new PerResolveLifetimeManager());

            // RBAC
            container.RegisterType<IPermissionsService, PermissionsService>(new PerResolveLifetimeManager());
            container.RegisterType<IFieldPermissionTypesService, FieldPermissionTypesService>(new PerResolveLifetimeManager());
            container.RegisterType<IFieldPermissionsService, FieldPermissionsService>(new PerResolveLifetimeManager());
            container.RegisterType<IChangesService, ChangesService>(new PerResolveLifetimeManager());
            container.RegisterType<IRolesService, RolesService>(new PerResolveLifetimeManager());

            // Business
            container.RegisterType<ISkillsService, SkillsService>(new PerResolveLifetimeManager());
            container.RegisterType<IOpportunitiesService, OpportunitiesService>(new PerResolveLifetimeManager());
            container.RegisterType<IAccountsService, AccountsService>(new PerResolveLifetimeManager());
            container.RegisterType<ICandidatesService, CandidatesService>(new PerResolveLifetimeManager());
            container.RegisterType<ISummaryService, SummaryService>(new PerResolveLifetimeManager());
        }
    }
}