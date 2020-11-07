using DuplicatesGui.ViewModel;

namespace DuplicatesGui.Interface
{
    public class Settings
    {
        public Settings()
        {
        }

        public Settings(string filter)
        {
            Filter = filter;
        }

        public string Filter { get; set; }
    }

    public interface ISettingsViewModel
    {
        ObservableProperty<Settings> SettingsSavedObservable
        {
            get;
        }
        string Filter
        {
            get;
            set;
        }

        void Activate();
    }
}
