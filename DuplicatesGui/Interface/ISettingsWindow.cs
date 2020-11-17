using System;

namespace DuplicatesGui.Interface
{
    public interface ISettingsWindow : IView
    {
        void Show();

        void Hide();

        bool IsVisible();
    }
}