using Microsoft.Practices.Unity;
using TechTracker.Repositories.Interfaces;

namespace TechTracker.Repositories.MongoDb
{
    public static class Unity
    {
        public static void Register(IUnityContainer container)
        {
            // Miscellaneous
            container.RegisterType<IEmailTemplatesRepository, EmailTemplatesRepository>(new PerResolveLifetimeManager());

              // Users
            container.RegisterType<IPreRegistrationRepository, PreRegistrationRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IRecoverPasswordRepository, RecoverPasswordRepository>(new PerResolveLifetimeManager());

            // RBAC
            container.RegisterType<IPermissionsRepository, PermissionsRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IFieldPermissionTypesRepository, FieldPermissionTypesRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IFieldPermissionsRepository, FieldPermissionsRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IChangesRepository, ChangesRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IRolesRepository, RolesRepository>(new PerResolveLifetimeManager());

            //Business
            container.RegisterType<ISkillsRepository, SkillsRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IAccountsRepository, AccountsRepository>(new PerResolveLifetimeManager());
            container.RegisterType<IOpportunitiesRepository, OpportunitiesRepository>(new PerResolveLifetimeManager());
            container.RegisterType<ICandidatesRepository, CandidatesRepository>(new PerResolveLifetimeManager());
            container.RegisterType<ISummaryRepository, SummaryRepository>(new PerResolveLifetimeManager());

        }
    }
}