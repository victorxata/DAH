using Microsoft.Practices.Unity;
using TechTracker.Common.Utils.Configuration;

namespace TechTracker.Common.Utils
{
    public static class Unity
    {
        public static void Register(IUnityContainer container)
        {
            container.RegisterType<IAppConfigManager, AppConfigManager>();
        }
    }
}