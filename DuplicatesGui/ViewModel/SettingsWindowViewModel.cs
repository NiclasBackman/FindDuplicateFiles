﻿using DuplicatesGui.Interface;
using DuplicatesGui.ViewModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DuplicatesGui.ViewModel
{

    public class SettingsWindowViewModel : INotifyPropertyChanged, IDataErrorInfo, ISettingsViewModel
    {
        private ICommand okCommand;
        private ICommand cancelCommand;
        private AttributeContainer<string> filterContainer;
        
        private readonly ISettingsService settingsService;

        public SettingsWindowViewModel(ISettingsService settingsService,
                                       ISettingsWindow settingsWindow)
        {
            okCommand = new CommandHandler(() => HandleOK(), () => CanExecuteOK);
            cancelCommand = new CommandHandler(() => HandleCancel(), () => CanExecuteCancel);
            filterContainer = new AttributeContainer<string>(string.Empty, new FilterValidationRule());
            UpdateViewModel(settingsService.QuerySettings());
            this.settingsService = settingsService;
            settingsWindow.DataContext = this;
        }

        public void Activate()
        {
            UpdateViewModel(settingsService.QuerySettings());
        }

        private bool IsChanged()
        {
            var settings = new Settings(filterContainer.Value);
            var savedSettings = settingsService.QuerySettings();
            var res = settings.Equals(savedSettings);
            return !settings.Equals(savedSettings);
        }

        void UpdateViewModel(Settings settings)
        {
            Filter = settings != null ? settings.Filter : "*.*";
        }

        private void HandleOK()
        {
            settingsService.SaveSettings(new Settings(filterContainer.Value));
        }

        public bool CanExecuteOK
        {
            get
            {
                var res = filterContainer.Rule.Validate(Filter, CultureInfo.InvariantCulture);
                return res.IsValid && IsChanged();
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
