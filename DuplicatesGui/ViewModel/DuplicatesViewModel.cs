using DuplicatesGui.Interface;
using DuplicatesLib;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace DuplicatesGui.ViewModel
{
    class DuplicatesViewModel : INotifyPropertyChanged, IDuplicatesViewModel
    {
        private ICommand browseCommand;
        private ICommand startCommand;
        private BackgroundWorker bgw;
        private string startPath;
        private int progressPercent;
        private ProgressBar progressBar;
        private bool m_CanCancel;
        private System.Windows.Input.Cursor m_Cursor;
        private SingleFileEntry m_CurrentItem;
        private bool m_State;
        private readonly IPreviewWindow previewWindow;
        private readonly ISettingsWindow settingsWindow;
        private readonly IDuplicateFinder queryService;
        private readonly IAboutBox aboutBox;
        private string filter;

        public DuplicatesViewModel(IPreviewWindow previewWindow,
                                   ISettingsWindow settingsWindow,
                                   IDuplicateFinder queryService,
                                   IAboutBox aboutWindows,
                                   ISettingsService settingsService)
        {
            Duplicates = new ObservableCollection<SingleFileEntry>();
            SelectedDuplicates = new ObservableCollection<SingleFileEntry>();
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(Duplicates);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Checksum");
            view.GroupDescriptions.Add(groupDescription);
            browseCommand = new CommandHandler(() => HandleBrowse(), () => CanExecuteBrowse);
            startCommand = new CommandHandler(() => HandleStart(), () => CanExecuteStart);
            ExportCommand = new CommandHandler(() => HandleExport(), () => CanExecuteExport);
            ImportCommand = new CommandHandler(() => HandleImport(), () => true);
            ShowPreviewCommand = new CommandHandler(() => HandleShowPreview(), () => CanExecuteShowPreview);
            ShowSettingsCommand = new CommandHandler(() => HandleShowSettings(), () => CanExecuteShowSettings);
            ShowAboutCommand = new CommandHandler(() => HandleAbout(), () => true);
            CancelCommand = new CommandHandler(() => HandleCancel(), () => CanExecuteCancel);
            RemoveItemCommand = new RelayCommand<SingleFileEntry>(HandleRemove, CanExecuteRemove);
            bgw = new BackgroundWorker();
            bgw.DoWork += DoWorkEvent;
            bgw.ProgressChanged += ProgressChangedEvent;
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            progressBar = new ProgressBar();
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Step = 1;
            progressBar.Visible = false;
            IsIdle = true;
            this.previewWindow = previewWindow;
            this.settingsWindow = settingsWindow;
            this.queryService = queryService;
            this.aboutBox = aboutWindows;
            settingsService.SettingsSavedObservable.Subscribe(HandleFilterSaved);
            Filter = settingsService.QuerySettings().Filter;
        }

        private void HandleFilterSaved(Settings value)
        {
            Filter = value.Filter;
        }

        private void ProgressChangedEvent(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercent = e.ProgressPercentage;
        }

        private void DoWorkEvent(object sender, DoWorkEventArgs e)
        {
            CanExecuteCancel = true;
            IsIdle = false;
            using (var c = new CursorHelper(this))
            {
                var res = queryService.QueryDuplicates(bgw, e, startPath, string.IsNullOrEmpty(filter) ? "*.*" : filter);
                System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { res.ForEach(x => Duplicates.Add(x)); }));
            }
            ProgressPercent = 0;
            CanExecuteCancel = false;
            IsIdle = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<SingleFileEntry> Duplicates { get; set; }

        public ObservableCollection<SingleFileEntry> SelectedDuplicates { get; set; }

        public System.Collections.IList SelectedItems
        {
            get
            {
                return SelectedDuplicates;
            }
            set
            {
                SelectedDuplicates.Clear();
                foreach (SingleFileEntry model in value)
                {
                    SelectedDuplicates.Add(model);
                }
            }
        }

        public System.Windows.Input.Cursor Cursor
        {
            get
            {
                return m_Cursor;
            }
            set
            {
                m_Cursor = value;
                OnPropertyChanged("Cursor");
            }
        }

        public bool IsIdle
        {
            get
            {
                return m_State;
            }
            set
            {
                m_State = value;
                OnPropertyChanged("IsIdle");
            }
        }

        public SingleFileEntry CurrentItem
        {
            get
            {
                return m_CurrentItem;
            }
            set
            {
                m_CurrentItem = value;
                OnPropertyChanged("CurrentItem");
            }
        }

        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                OnPropertyChanged("Filter");
            }
        }

        public string StartPath
        {
            get
            {
                return startPath;
            }
            set
            {
                startPath = value;
                OnPropertyChanged("StartPath");
            }
        }

        public int ProgressPercent
        {
            get
            {
                return progressPercent;
            }
            set
            {
                progressPercent = value;
                OnPropertyChanged("ProgressPercent");
            }
        }

        private void HandleAbout()
        {
            if(aboutBox.IsVisible())
            {
                return;
            }

            aboutBox.Show();
        }

        private void HandleShowSettings()
        {
            settingsWindow.Show();
        }

        private void HandleShowPreview()
        {
            previewWindow.Show();
        }

        private void HandleBrowse()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                StartPath = dialog.SelectedPath;
            }
        }

        private void HandleExport()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Export duplicates to file";
            saveFileDialog.Filter = "Serialized object|*.xml";
            saveFileDialog.FileName = "duplicate_files.xml";
            var res = saveFileDialog.ShowDialog();
            if(res == DialogResult.OK && saveFileDialog.FileName != string.Empty)
            {
                using (StreamWriter myWriter = new StreamWriter(saveFileDialog.FileName, false))
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(ObservableCollection<SingleFileEntry>));
                    mySerializer.Serialize(myWriter, Duplicates);
                }
            }
        }

        private void HandleImport()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Import duplicates from file";
            var res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                using (var c = new CursorHelper(this))
                {
                    using (Stream reader = new FileStream(dlg.FileName, FileMode.Open))
                    {
                        XmlSerializer mySerializer = new XmlSerializer(typeof(ObservableCollection<SingleFileEntry>));
                        var items = (ObservableCollection<SingleFileEntry>)mySerializer.Deserialize(reader);
                        Duplicates.Clear();
                        foreach (var i in items)
                        {
                            Duplicates.Add(i);
                        }
                    }
                }
            }
        }

        private void HandleStart()
        {
            Duplicates.Clear();
            bgw.RunWorkerAsync(progressBar);
        }

        private void HandleCancel()
        {
            bgw.CancelAsync();
        }

        private void HandleRemove(SingleFileEntry item)
        {
            var items = SelectedItems.Cast<SingleFileEntry>().ToList();
            CurrentItem = null;
            using (new CursorHelper(this))
            {
                foreach (var selItem in items)
                {
                    var i = Duplicates.Where(x => x.FilePath == selItem.FilePath).FirstOrDefault();

                    Duplicates.Remove(i);
                    GC.Collect(); System.GC.WaitForPendingFinalizers();
                    FileSystem.DeleteFile(i.FilePath,
                                          UIOption.AllDialogs,
                                          RecycleOption.SendToRecycleBin,
                                          UICancelOption.ThrowException);
                }
            }
        }

        public ICommand ShowAboutCommand { get; }
        
        public ICommand ShowSettingsCommand { get; }

        public ICommand ShowPreviewCommand { get; }

        public ICommand ImportCommand { get; }

        public ICommand ExportCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand BrowseCommand
        {
            get
            {
                return browseCommand;
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return startCommand;
            }
        }

        public ICommand CancelCommand { get; }

        public bool CanExecuteExport
        {
            get
            {
                return Duplicates.Count > 0;
            }
        }

        public bool CanExecuteShowPreview
        {
            get
            {
                return !previewWindow.IsVisible();
            }
        }
        public bool CanExecuteShowSettings
        {
            get
            {
                return !settingsWindow.IsVisible();
            }
        }

        public bool CanExecuteBrowse
        {
            get
            {
                return true;
            }
        }

        public bool CanExecuteStart
        {
            get
            {
                return !string.IsNullOrEmpty(startPath) && Directory.Exists(startPath) && !CanExecuteCancel;
            }
        }

        public bool CanExecuteCancel
        {
            get
            {
                return m_CanCancel;
            }
            set
            {
                m_CanCancel = value;
                OnPropertyChanged("CanCancel");
            }
        }

        private bool CanExecuteRemove(SingleFileEntry obj)
        {
            var nonExistentItems = SelectedItems.Cast<SingleFileEntry>().ToList().Where(f => File.Exists(f.FilePath) == false).ToList();
            return Duplicates.Count > 0 && obj != null && nonExistentItems.Count() == 0;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
