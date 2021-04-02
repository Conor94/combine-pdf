using CombinePdf_GUI.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace CombinePdf_GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register dependencies
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
        }
    }
}
