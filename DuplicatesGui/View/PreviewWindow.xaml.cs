using DuplicatesGui.Interface;
using System.ComponentModel;
using System.Windows;

namespace DuplicatesGui.View
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window, IPreviewWindow
    {
        public PreviewWindow()
        {
            InitializeComponent();
            Closing += HandleWindowClosed;
        }

        private void HandleWindowClosed(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


        bool IPreviewWindow.IsVisible()
        {
            return this.Visibility == Visibility.Visible;
        }
    }
}
