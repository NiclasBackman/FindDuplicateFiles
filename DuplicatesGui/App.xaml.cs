using DuplicatesGui.Module;
using DuplicatesGui.View;
using DuplicatesLib.Module;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace DuplicatesGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(GuiModule));
            moduleCatalog.AddModule(typeof(DuplicateServiceModule));
            base.ConfigureModuleCatalog(moduleCatalog);
            base.InitializeModules();
        }

        protected override Window CreateShell()
        {
            return new MainWindow(Container);
        }
    }
}
