using DuplicatesGui.Interface;
using DuplicatesGui.ViewModel;
using DuplicatesLib;
using Prism.Ioc;
using System.ComponentModel;
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
            container.Resolve<IPreviewViewModel>();
            previewWindow = container.Resolve<IPreviewWindow>();
            previewWindow.Hide();
            var settingsVm = container.Resolve<ISettingsViewModel>();
            settingsWindow = container.Resolve<ISettingsWindow>();
            var queryService = container.Resolve<IDuplicateFinder>();
            settingsWindow.Hide();
            this.Closing += HandleWindowClosed;
            DataContext = new DuplicatesViewModel(previewWindow, settingsWindow, queryService, container.Resolve<IAboutBox>(), container.Resolve<ISettingsService>());            
        }

        private void HandlePreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
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

        private void AppExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HandleWindowClosed(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && Directory.Exists(files[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }

        }

        private void TextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            object text = e.Data.GetData(DataFormats.FileDrop);
            Button btn = sender as Button;
            if (btn != null)
            {
                var vm = DataContext as DuplicatesViewModel;
                vm.StartPath = string.Format("{0}", ((string[])text)[0]);
            }
        }
    }
}
