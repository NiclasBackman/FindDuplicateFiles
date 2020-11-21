namespace DuplicatesGui.Interface
{
    public interface IPreviewWindow : IView
    {
        void Show();

        void Hide();

        bool IsVisible();
    }
}