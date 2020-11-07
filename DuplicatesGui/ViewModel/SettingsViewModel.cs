using DuplicatesGui.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Serialization;

namespace DuplicatesGui.ViewModel
{

    public class SettingsViewModel : INotifyPropertyChanged, IDataErrorInfo, ISettingsViewModel
    {
        private ICommand okCommand;
        private ICommand cancelCommand;
        private AttributeContainer<string> filterContainer;
        private List<IObserver<string>> observers;
        private readonly ObservableProperty<Settings> settingsSavedObservable;
        private readonly string settingsFileName;

        public SettingsViewModel()
        {
            observers = new List<IObserver<string>>();
            okCommand = new CommandHandler(() => HandleOK(), () => CanExecuteOK);
            cancelCommand = new CommandHandler(() => HandleCancel(), () => CanExecuteCancel);
            filterContainer = new AttributeContainer<string>(string.Empty, new FilterValidationRule());
            settingsSavedObservable = new ObservableProperty<Settings>();
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Zalcin");
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            settingsFileName = Path.Combine(basePath, "Settings.xml");

            LoadData();
        }

        public void Activate()
        {
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(settingsFileName))
            {
                using (Stream reader = new FileStream(settingsFileName, FileMode.Open))
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(Settings));
                    var items = (Settings)mySerializer.Deserialize(reader);
                    Filter = items.Filter;
                }
            }
            else
            {
                Filter = "*.*";
            }
        }

        private void HandleOK()
        {
            var settings = new Settings(filterContainer.Value);
            settingsSavedObservable.Publish(settings);
            using (StreamWriter myWriter = new StreamWriter(settingsFileName, false))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(Settings));
                mySerializer.Serialize(myWriter, settings);
            }
        }

        public bool CanExecuteOK
        {
            get
            {
                var res = filterContainer.Rule.Validate(Filter, CultureInfo.InvariantCulture);
                return res.IsValid;
            }
        }

        private void HandleCancel()
        {
        }

        public bool CanExecuteCancel
        {
            get
            {
                return true;
            }
        }

        public ICommand OKCommand
        {
            get
            {
                return okCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return cancelCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Filter
        {
            get
            {
                return filterContainer.Value;
            }
            set
            {
                filterContainer.Value = value;
                OnPropertyChanged("Filter");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public string Error
        {
            get { return "...."; }
        }

        public ObservableProperty<Settings> SettingsSavedObservable 
        {
            get => settingsSavedObservable;
        }

        /// <summary>
        /// Will be called for each and every property when ever its value is changed
        /// </summary>
        /// <param name="columnName">Name of the property whose value is changed</param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                return Validate(columnName);
            }
        }

        private string Validate(string propertyName)
        {
            // Return error message if there is error on else return empty or null string
            string validationMessage = string.Empty;
            switch (propertyName)
            {
                case "Filter": // property name
                               // TODO: Check validiation condition
                    var res = filterContainer.Rule.Validate(Filter, CultureInfo.InvariantCulture);
                    if(!res.IsValid)
                    {
                        validationMessage = res.ErrorContent as string;
                    }
                    break;
            }

            return validationMessage;
        }
    }
}
