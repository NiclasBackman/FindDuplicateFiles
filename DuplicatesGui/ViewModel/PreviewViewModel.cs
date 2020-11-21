using DuplicatesGui.Interface;
using System.Windows.Input;
using Unity;

namespace DuplicatesGui.ViewModel
{
    public class PreviewViewModel : IPreviewViewModel
    {
        private readonly IPreviewWindow previewWindow;

        [InjectionConstructor]
        public PreviewViewModel(IPreviewWindow previewWindow)
        {
            CloseCommand = new CommandHandler(() => HandleClose(), () => CanExecuteClose);
            this.previewWindow = previewWindow;
            previewWindow.DataContext = this;
        }

        public bool CanExecuteClose { get { return true; } }

        public ICommand CloseCommand { get; }

        private void HandleClose()
        {
            previewWindow.Hide();
        }
    }
}
