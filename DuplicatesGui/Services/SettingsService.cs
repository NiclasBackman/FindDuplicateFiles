using DuplicatesGui.Interface;
using DuplicatesGui.ViewModel;
using System;
using System.IO;
using System.Xml.Serialization;

namespace DuplicatesGui.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string settingsFileName;

        public SettingsService()
        {
            SettingsSavedObservable = new ObservableProperty<Settings>();
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zalcin");
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            settingsFileName = Path.Combine(basePath, "Settings.xml");
        }

        public ObservableProperty<Settings> SettingsSavedObservable { get; }

        public Settings QuerySettings()
        {
            if (File.Exists(settingsFileName))
            {
                using (Stream reader = new FileStream(settingsFileName, FileMode.Open))
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Settings));
                    var items = (Settings)mySerializer.Deserialize(reader);
                    return items;
                }
            }
            else
            {
                return null;
            }
        }

        public void SaveSettings(Settings settings)
        {
            using (StreamWriter myWriter = new StreamWriter(settingsFileName, false))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Settings));
                mySerializer.Serialize(myWriter, settings);
            }

            SettingsSavedObservable.Publish(settings);
        }
    }
}
