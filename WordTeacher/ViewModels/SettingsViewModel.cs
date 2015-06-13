using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WordTeacher.Commands;
using WordTeacher.Models;

namespace WordTeacher.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged, ICloseable
    {
        private bool _areUnsavedChanges;
        private ICommand _saveCommand;
        private ICommand _exitCommand;
        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>();

        public List<TranslationItem> SavedTranslationItems = new List<TranslationItem>();

        public bool AreUnsavedChanges
        {
            get { return _areUnsavedChanges; }
            set
            {
                _areUnsavedChanges = value;
                OnPropertyChanged("AreUnsavedChanges");
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

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new CommandHandler(SaveSettings, true));
            }
        }

        public ICommand ExitCommand
        {
            get 
            {
                return _exitCommand ?? (_exitCommand = new CommandHandler(CloseWindow, true));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> RequestClose;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        private void SaveSettings()
        {
            SavedTranslationItems = new List<TranslationItem>(TranslationItems);
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

        public bool ExitSettings()
        {
            if (AreUnsavedChanges && ShowExitDialog() != MessageBoxResult.Yes)
                return false;
            
            TranslationItems = new ObservableCollection<TranslationItem>(SavedTranslationItems);
            return true;
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
