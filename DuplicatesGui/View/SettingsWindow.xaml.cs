using DuplicatesGui.Interface;
using System.ComponentModel;
using System.Windows;

namespace DuplicatesGui.View
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, ISettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Closing += HandleWindowClosed;
        }

        bool ISettingsWindow.IsVisible()
        {
            return this.Visibility == Visibility.Visible;
        }

        private void HandleWindowClosed(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void HandleOkPressed(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void HandleCancelPressed(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
