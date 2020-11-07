using System;

namespace DuplicatesGui.Interface
{
    public interface ISettingsWindow
    {
        void Show();

        void Hide();

        bool IsVisible();
    }
}