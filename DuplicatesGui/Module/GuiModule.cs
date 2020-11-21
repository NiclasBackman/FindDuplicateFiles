using DuplicatesGui.Interface;
using DuplicatesGui.Services;
using DuplicatesGui.View;
using DuplicatesGui.ViewModel;
using Prism.Ioc;
using Prism.Modularity;

namespace DuplicatesGui.Module
{

    public class GuiModule : IModule
    {
        private IContainerProvider container = null;

        public void OnInitialized(IContainerProvider containerProvider)
        {
            container = containerProvider;
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
            containerRegistry.RegisterSingleton<IPreviewWindow, PreviewWindow>();
            containerRegistry.RegisterSingleton<IPreviewViewModel, PreviewViewModel>();
            containerRegistry.RegisterSingleton<ISettingsWindow, SettingsWindow>();
            containerRegistry.RegisterSingleton<ISettingsViewModel, SettingsViewModel>();
            containerRegistry.RegisterSingleton<IAboutBox, AboutBox>();
        }
    }
}
