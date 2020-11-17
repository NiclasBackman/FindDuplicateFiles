using DuplicatesGui.Interface;
using DuplicatesGui.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicatesGui.Interface
{
    public interface ISettingsService
    {
        ObservableProperty<Settings> SettingsSavedObservable
        {
            get;
        }

        Settings QuerySettings();

        void SaveSettings(Settings settings);
    }
}
