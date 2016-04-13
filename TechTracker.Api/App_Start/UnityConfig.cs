using System;
using Microsoft.Practices.Unity;

namespace TechTracker.Api
{
    /// <summary>
    ///     Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        ///     There is no need to register concrete types such as controllers or API controllers (unless you want to
        ///     change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // Application Configuration IoC Services
            TechTracker.Common.Utils.Unity.Register(container);

            // Logging
            TechTracker.Common.Tracer.Unity.Register(container);

            // TechTracker IoC Services
            Services.Unity.Register(container);

            // TechTracker IoC Repositories
            Repositories.MongoDb.Unity.Register(container);

            // Data Models
            Domain.Data.Models.Unity.Register(container);
        }

        #region Unity Container

        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        ///     Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        #endregion
    }
}