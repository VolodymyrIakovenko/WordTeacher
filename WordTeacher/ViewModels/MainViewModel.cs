using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using WordTeacher.Commands;
using WordTeacher.Extensions;
using WordTeacher.Models;
using WordTeacher.Properties;
using WordTeacher.Utilities;
using WordTeacher.Views;

namespace WordTeacher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged  
    {
        private double _positionX;
        private double _positionY;
        private bool _isSettingsOpened;
        private bool _isHidden;
        private int _translationItemIndex;

        private ICommand _nextItemCommand;
        private ICommand _closeCommand;
        private ICommand _settingsCommand;

        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>(); 

        public MainViewModel()
        {
            SettingsUtility.CheckSettingsFolder();
            TranslationItems = new ObservableCollection<TranslationItem>(SettingsUtility.Load());

            ArrangeWindowPosition();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Position of the window on the X axis.
        /// </summary>
        public double PositionX
        {
            get { return _positionX; }
            set
            {
                _positionX = value;
                OnPropertyChanged("PositionX");
            }
        }

        /// <summary>
        /// Position of the window on the Y axis.
        /// </summary>
        public double PositionY
        {
            get { return _positionY; }
            set
            {
                _positionY = value;
                OnPropertyChanged("PositionY");
            }
        }

        /// <summary>
        /// Position of the window on the Y axis.
        /// </summary>
        public bool IsSettingsOpened
        {
            get { return _isSettingsOpened; }
            set
            {
                _isSettingsOpened = value;
                OnPropertyChanged("IsSettingsOpened");
            }
        }

        /// <summary>
        /// Is window hidden.
        /// </summary>
        public bool IsHidden
        {
            get { return _isHidden; }
            set
            {
                _isHidden = value;
                OnPropertyChanged("IsHidden");
                OnPropertyChanged("CollapseImage");
            }
        }

        public string CollapseImage
        {
            get { return IsHidden ? "../Resources/expand_arrow.png" : "../Resources/collapse_arrow.png"; }
        }

        /// <summary>
        /// Current shown word and its tranlsation.
        /// </summary>
        public TranslationItem CurrentTranslationItem
        {
            get { return TranslationItems[_translationItemIndex]; }
        }

        /// <summary>
        /// The list of words and their translations.
        /// </summary>
        public ObservableCollection<TranslationItem> TranslationItems
        {
            get { return _translationItems; }
            set
            {
                _translationItems = value;
                OnPropertyChanged("TranslationItems");
                OnPropertyChanged("CurrentTranslationItem");
            }
        }

        public ICommand NextItemCommand
        {
            get
            {
                return _nextItemCommand ?? (_nextItemCommand = new CommandHandler(NextItem, true));
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                return _settingsCommand ?? (_settingsCommand = new CommandHandler(OpenSettings, true));
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new CommandHandler(CloseApplication, true));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Assign position to the window.
        /// </summary>
        private void ArrangeWindowPosition()
        {
            var topCenterPoint = ScreenUtility.GetTopCenterPoint();
            PositionX = topCenterPoint.X;
            PositionY = topCenterPoint.Y;
        }

        private void NextItem()
        {
            if (TranslationItems.Count <= 1)
                return;

            if (Settings.Default.NextRandom)
            {
                var oldValue = _translationItemIndex;
                var rnd = new Random();
                while (oldValue == _translationItemIndex)
                {
                    _translationItemIndex = rnd.Next(0, TranslationItems.Count);
                }
            }
            else
            {
                _translationItemIndex++;
                if (_translationItemIndex >= TranslationItems.Count)
                    _translationItemIndex = 0;
            }
            OnPropertyChanged("CurrentTranslationItem");
        }

        private void OpenSettings()
        {
            var settingsView = new SettingsView();

            settingsView.Closed += SettingsViewOnClosed;
            settingsView.Show();

            IsSettingsOpened = true;
        }

        private void SettingsViewOnClosed(object sender, EventArgs eventArgs)
        {
            var settingsView = sender as SettingsView;
            if (settingsView != null)
            {
                // Copy items from settings.
                var settingsViewModel = (SettingsViewModel)settingsView.DataContext;
                TranslationItems = new ObservableCollection<TranslationItem>(settingsViewModel.SavedTranslationItems.Clone());                    
            }

            IsSettingsOpened = false;
        }

        private static void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
