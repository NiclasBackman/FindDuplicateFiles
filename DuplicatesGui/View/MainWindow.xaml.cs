using DuplicatesGui.Interface;
using DuplicatesGui.ViewModel;
using DuplicatesLib;
using Prism.Ioc;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DuplicatesGui.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPreviewWindow previewWindow;
        private ISettingsWindow settingsWindow;

        public MainWindow(IDuplicatesViewModel viewModel)
        {
            this.DataContext = viewModel;
        }

        public MainWindow(IContainerProvider container)
        {
            InitializeComponent();
            previewWindow = container.Resolve<IPreviewWindow>();
            previewWindow.Hide();
            var settingsVm = container.Resolve<ISettingsViewModel>();
            settingsWindow = container.Resolve<ISettingsWindow>();
            var queryService = container.Resolve<IDuplicateFinder>();
            settingsWindow.Hide();
            DataContext = new DuplicatesViewModel(previewWindow, settingsWindow, queryService, container.Resolve<IAboutBox>(), container.Resolve<ISettingsService>());
        }

        private void listView_Click(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                var path = (item as SingleFileEntry).FilePath;
                if (File.Exists(path))
                {
                    var wnd = previewWindow as PreviewWindow;
                    wnd.previewControl.FileName = path;
                    wnd.Title = "Preview [" + path + "]";
                }
            }
        }

        private void listView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                var path = (item as SingleFileEntry).FilePath;
                (previewWindow as PreviewWindow).previewControl.FileName = path;
                lvDuplicates.SelectedItem = item;
            }
        }

        private void listView_Double_Click(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                var path = (item as SingleFileEntry).FilePath;
                System.Diagnostics.Process.Start(path);
            }
        }

        private void HandleWindowClicked(object sender, MouseButtonEventArgs e)
        {
            //var o = sender as ListViewItem;
            //if (o == null)
            //{
            //    this.lvDuplicates.SelectedItem = null;
            //}
            //Point pt = e.GetPosition((UIElement)sender);
            //HitTestResult result = VisualTreeHelper.HitTest(this, pt);
            ////HitTestResult hitTestResult = VisualTreeHelper.HitTest(sender, e.GetPosition(sender as IInputElement));
            //Control controlUnderMouse = result.VisualHit.GetType;
            
            //if (controlUnderMouse.GetType() != typeof(ListBoxItem))
            //    lvDuplicates.SelectedItem = null;
        }

        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
