using Prism.Ioc;
using Prism.Modularity;

namespace DuplicatesLib.Module
{
    public class DuplicateServiceModule : IModule
    {
        private IContainerProvider container = null;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            container = containerProvider;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDuplicateFinder, DuplicateFinder>();
        }
    }
}
