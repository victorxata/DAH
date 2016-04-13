using log4net;
using Microsoft.Practices.Unity;

namespace TechTracker.Common.Tracer
{
    public static class Unity
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<ILog, LoggerWrapper>(new ContainerControlledLifetimeManager());
        }
    }
}