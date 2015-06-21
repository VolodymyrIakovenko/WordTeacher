using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using WordTeacher.Commands;
using WordTeacher.Extensions;
using WordTeacher.Models;
using WordTeacher.Properties;
using WordTeacher.Utilities;

namespace WordTeacher.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged, ICloseable
    {
        private bool _areUnsavedChanges;
        private bool _randomSetting;
        private bool _autoStartSetting;
        private bool _autoChangeSetting;
        private int _changeInMinutesSetting;
        private TranslationItem _selectedTranslationItem;
        private TranslationItem _shownTranslationItem;

        private ICommand _addWordCommand;
        private ICommand _deleteWordCommand;
        private ICommand _chooseWordCommand;
        private ICommand _saveCommand;
        private ICommand _exitCommand;
        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>();

        private List<TranslationItem> _savedTranslationItems = new List<TranslationItem>();

        public SettingsViewModel()
        {
            SettingsUtility.CheckSettingsFolder();
            TranslationItems = new ObservableCollection<TranslationItem>(SettingsUtility.Load());
            SavedTranslationItems = new List<TranslationItem>(TranslationItems.Clone());
            
            RandomSetting = Settings.Default.NextRandom;
            AutoStartSetting = Settings.Default.AutoStart;
            AutoChangeSetting = Settings.Default.AutoChange;
            ChangeInMinutesSetting = Settings.Default.ChangeInMinutes;

            UpdateIfAnyNewSettings();
        }

        public bool AreUnsavedChanges
        {
            get { return _areUnsavedChanges; }
            set
            {
                _areUnsavedChanges = value;
                OnPropertyChanged("AreUnsavedChanges");
                OnPropertyChanged("IsUpdateWordEnabled");
            }
        }

        public bool RandomSetting
        {
            get { return _randomSetting; }
            set
            {
                _randomSetting = value;
                OnPropertyChanged("RandomSetting");
                UpdateIfAnyNewSettings();
            }
        }

        public bool AutoStartSetting
        {
            get { return _autoStartSetting; }
            set
            {
                _autoStartSetting = value;
                OnPropertyChanged("AutoStartSetting");
                UpdateIfAnyNewSettings();
            }
        }

        public bool AutoChangeSetting
        {
            get { return _autoChangeSetting; }
            set
            {
                _autoChangeSetting = value;
                OnPropertyChanged("AutoChangeSetting");
                UpdateIfAnyNewSettings();
            }
        }

        public int ChangeInMinutesSetting
        {
            get { return _changeInMinutesSetting; }
            set
            {
                _changeInMinutesSetting = value;
                OnPropertyChanged("ChangeInMinutesSetting");
                UpdateIfAnyNewSettings();
            }
        }

        public bool IsDeleteWordEnabled
        {
            get { return SelectedTranslationItem != null; }
        }

        public bool IsUpdateWordEnabled
        {
            get 
            { 
                return SelectedTranslationItem != null && 
                       !SelectedTranslationItem.Equals(ShownTranslationItem) && 
                       !AreUnsavedChanges; 
            }
        } 

        public TranslationItem SelectedTranslationItem
        {
            get { return _selectedTranslationItem; }
            set
            {
                _selectedTranslationItem = value;
                OnPropertyChanged("IsDeleteWordEnabled");
                OnPropertyChanged("IsUpdateWordEnabled");
            }
        }

        public TranslationItem ShownTranslationItem
        {
            get { return _shownTranslationItem; }
            set
            {
                _shownTranslationItem = value;
                OnPropertyChanged("IsUpdateWordEnabled");
            }
        } 

        public ObservableCollection<TranslationItem> TranslationItems
        {
            get { return _translationItems; }
            set
            {
                _translationItems = value;
                OnPropertyChanged("TranslationItems");
            }
        }

        public List<TranslationItem> SavedTranslationItems
        {
            get { return _savedTranslationItems; }
            set
            {
                _savedTranslationItems = value;
                var handler = WordsSaved;
                if (handler != null)
                    handler.Invoke(this, _savedTranslationItems);
            }
        }

        public ICommand AddWordCommand
        {
            get { return _addWordCommand ?? (_addWordCommand = new CommandHandler(AddWord, true)); }
        }

        public ICommand DeleteWordCommand
        {
            get { return _deleteWordCommand ?? (_deleteWordCommand = new CommandHandler(DeleteWord, true)); }
        }

        public ICommand ChooseWordCommand
        {
            get { return _chooseWordCommand ?? (_chooseWordCommand = new CommandHandler(ChooseWord, true)); }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new CommandHandler(SaveSettings, true)); }
        }

        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new CommandHandler(CloseWindow, true)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> RequestClose;
        public event EventHandler<TranslationItem> SelectedWordUpdated;
        public event EventHandler<List<TranslationItem>> WordsSaved;

        /// <summary>
        /// Exits the settings window.
        /// </summary>
        /// <returns></returns>
        public bool ExitSettings()
        {
            if (AreUnsavedChanges && ShowExitDialog() == MessageBoxResult.No)
                return false;

            RollbackSettings();
            return true;
        }

        /// <summary>
        /// Update the flag, which defines if there are any unsaved settings.
        /// </summary>
        public void UpdateIfAnyNewSettings()
        {
            AreUnsavedChanges = CheckIfAnyNewSettings();
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        private void AddWord()
        {
            if (TranslationItems == null)
                TranslationItems = new ObservableCollection<TranslationItem>();

            TranslationItems.Add(new TranslationItem());
            SelectedTranslationItem = TranslationItems[TranslationItems.Count - 1];
            UpdateIfAnyNewSettings();
        }

        private void DeleteWord()
        {
            if (TranslationItems == null || SelectedTranslationItem == null)
                return;

            if (!TranslationItems.Remove(SelectedTranslationItem))
                return;
    
            SelectedTranslationItem = null;
            UpdateIfAnyNewSettings();
        }

        private void ChooseWord()
        {
            var handler = SelectedWordUpdated;
            if (handler != null)
                handler.Invoke(this, SelectedTranslationItem);

            SelectedTranslationItem = null;
        }

        /// <summary>
        /// Detects if new settings don't equal to saved ones.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAnyNewSettings()
        {
            return RandomSetting != Settings.Default.NextRandom ||
                   AutoStartSetting != Settings.Default.AutoStart ||
                   AutoChangeSetting != Settings.Default.AutoChange ||
                   ChangeInMinutesSetting != Settings.Default.ChangeInMinutes ||
                   !TranslationItems.SequenceEqual(SavedTranslationItems);
        }

        /// <summary>
        /// Rollbacks all settings to saved ones.
        /// </summary>
        private void RollbackSettings()
        {
            TranslationItems = new ObservableCollection<TranslationItem>(SavedTranslationItems.Clone());
            RandomSetting = Settings.Default.NextRandom;
            AutoStartSetting = Settings.Default.AutoStart;
            AutoChangeSetting = Settings.Default.AutoChange;
            ChangeInMinutesSetting = Settings.Default.ChangeInMinutes;
            
            AreUnsavedChanges = false;
        }

        /// <summary>
        /// Save all settings.
        /// </summary>
        private void SaveSettings()
        {
            SettingsUtility.Save(new List<TranslationItem>(TranslationItems));
            Settings.Default.NextRandom = RandomSetting;
            Settings.Default.AutoStart = AutoStartSetting;
            Settings.Default.AutoChange = AutoChangeSetting;
            Settings.Default.ChangeInMinutes = ChangeInMinutesSetting;
            Settings.Default.Save();

            SavedTranslationItems = new List<TranslationItem>(TranslationItems.Clone());
            AreUnsavedChanges = false;
        }
    
        private void CloseWindow()
        {
            if (!ExitSettings())
                return;

            var handler = RequestClose;
            if (handler != null)
                handler.Invoke(this, new EventArgs());
        }

        private MessageBoxResult ShowExitDialog()
        {
            const string messageBoxText = "Are you sure want to exit settings without saving changes?";
            const string caption = "Warning";

            const MessageBoxButton buttonMessageBox = MessageBoxButton.YesNo;
            const MessageBoxImage iconMessageBox = MessageBoxImage.Warning;

            return MessageBox.Show(messageBoxText, caption, buttonMessageBox, iconMessageBox);
        }
    }
}
