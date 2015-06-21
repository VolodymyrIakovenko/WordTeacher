using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WordTeacher.Commands;
using WordTeacher.Extensions;
using WordTeacher.Models;
using WordTeacher.Properties;
using WordTeacher.Utilities;
using WordTeacher.Views;

namespace WordTeacher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private const int MillisecondsInMinute = 60 * 1000;

        private readonly Timer _autoChangeTimer;

        private double _positionX;
        private double _positionY;
        private double _height;
        private double _width;
        private double _dragginPosX;
        private double _dragginPosY;
        private bool _isSettingsOpened;
        private bool _isHidden;
        private int _translationItemIndex;
        private SettingsViewModel _settingsViewModel;

        private ICommand _nextItemCommand;
        private ICommand _closeCommand;
        private ICommand _settingsCommand;

        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>(); 

        public MainViewModel()
        {
            // Load translation items from appdata files.
            SettingsUtility.CheckSettingsFolder();
            TranslationItems = new ObservableCollection<TranslationItem>(SettingsUtility.Load());

            // Subscribe to settings changes.
            Settings.Default.SettingsSaving += DefaultSettingsOnSettingsSaving;

            // Initialize word auto change timer.
            _autoChangeTimer = new Timer();
            _autoChangeTimer.Elapsed += (sender, args) => NextItem();
            UpdateAutoChangeSettings();

            // Adjust the main view.
            Height = 62;
            Width = 300;
            ArrangeWindowPosition();
        }

        private void DefaultSettingsOnSettingsSaving(object sender, CancelEventArgs cancelEventArgs)
        {
            UpdateAutoChangeSettings();
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
        /// Height of the window.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Width of the window.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
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

        /// <summary>
        /// Image that is shown on collapse/expand button.
        /// </summary>
        public string CollapseImage
        {
            get { return IsHidden ? "../Resources/expand_arrow.png" : "../Resources/collapse_arrow.png"; }
        }

        /// <summary>
        /// Current shown word and its tranlsation.
        /// </summary>
        public TranslationItem CurrentTranslationItem
        {
            get { return TranslationItems.Any() ? TranslationItems[_translationItemIndex] : new TranslationItem(string.Empty, string.Empty); }
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
            get { return _nextItemCommand ?? (_nextItemCommand = new CommandHandler(NextItem, true)); }
        }

        public ICommand SettingsCommand
        {
            get { return _settingsCommand ?? (_settingsCommand = new CommandHandler(OpenSettings, true)); }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new CommandHandler(CloseApplication, true)); }
        }

        public void Dispose()
        {
            _autoChangeTimer.Dispose();
        }

        public void WindowLoaded(IntPtr handle)
        {
            var hwndSource = HwndSource.FromHwnd(handle);
            if (hwndSource != null)
                hwndSource.AddHook(HwndSourceHookHandler);
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

            UpdateCurrentItem();
        }

        private void UpdateCurrentItem()
        {
            OnPropertyChanged("CurrentTranslationItem");
            if (_settingsViewModel != null)
                _settingsViewModel.ShownTranslationItem = CurrentTranslationItem;
        }

        private void UpdateAutoChangeSettings()
        {
            var newInterval = Settings.Default.ChangeInMinutes * MillisecondsInMinute;
            if (Math.Abs(_autoChangeTimer.Interval/MillisecondsInMinute - newInterval) >= 1)
            {
                if (_autoChangeTimer.Enabled)
                {
                    _autoChangeTimer.Enabled = false;
                    _autoChangeTimer.Interval = newInterval;
                    _autoChangeTimer.Enabled = true;
                }
                else
                {
                    _autoChangeTimer.Interval = newInterval;
                }
            }

            _autoChangeTimer.Enabled = Settings.Default.AutoChange;
        }

        private void OpenSettings()
        {
            var settingsView = new SettingsView();

            _settingsViewModel = (SettingsViewModel)settingsView.DataContext;
            _settingsViewModel.ShownTranslationItem = CurrentTranslationItem;
            _settingsViewModel.SelectedWordUpdated += SettingsViewModelOnSelectedWordUpdated;
            _settingsViewModel.WordsSaved += SettingsViewModelOnWordsSaved;

            settingsView.Closed += SettingsViewOnClosed;
            settingsView.Show();

            IsSettingsOpened = true;
        }

        private void SettingsViewModelOnSelectedWordUpdated(object sender, TranslationItem translationItem)
        {
            if (_settingsViewModel == null)
                return;

            var newIndex = TranslationItems.IndexOf(translationItem);
            if (newIndex < 0 || newIndex == _translationItemIndex || newIndex >= TranslationItems.Count)
                return;

            _translationItemIndex = newIndex;
            UpdateCurrentItem();

            // Restart the auto change timer.
            if (!_autoChangeTimer.Enabled) 
                return;

            _autoChangeTimer.Enabled = false;
            _autoChangeTimer.Enabled = true;
        }

        private void SettingsViewModelOnWordsSaved(object sender, List<TranslationItem> translationItems)
        {
            if (_translationItemIndex >= translationItems.Count)
            {
                _translationItemIndex = 0;
                UpdateCurrentItem();
            }

            // Copy items from settings.
            TranslationItems = new ObservableCollection<TranslationItem>(_settingsViewModel.SavedTranslationItems.Clone());
        }

        private void SettingsViewOnClosed(object sender, EventArgs eventArgs)
        {
            if (_settingsViewModel != null )
            {
                _settingsViewModel.SelectedWordUpdated -= SettingsViewModelOnSelectedWordUpdated;
            }

            IsSettingsOpened = false;
        }

        public IntPtr HwndSourceHookHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WmMoving:
                {
                    var rectangle = (NativeMethods.MoveRectangle)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MoveRectangle));

                    
                    rectangle.Top = (int) PositionY;
                    rectangle.Bottom = (int) (rectangle.Top + Height);
                    _dragginPosY = rectangle.Top;

                    if (rectangle.Left < ScreenUtility.GetWorkAreaLeft())
                    {
                        rectangle.Left = (int) ScreenUtility.GetWorkAreaLeft();
                        rectangle.Right = (int) Width;
                    }
                    if (rectangle.Right > ScreenUtility.GetWorkAreaRight())
                    {
                        rectangle.Right = (int) ScreenUtility.GetWorkAreaRight();
                        rectangle.Left = (int) (rectangle.Right - Width);
                    }
                    _dragginPosX = rectangle.Left;

                    Marshal.StructureToPtr(rectangle, lParam, true);
                    break;
                }
                case NativeMethods.WmExitsizemove:
                {
                    // Fixes a problem when after dragging the window can't be shown outside of screen boundaries.
                    var top = IsHidden ? _dragginPosY : ScreenUtility.GetTopYAxisValue();
                    NativeMethods.SetWindowPos(hwnd, 0, (int) _dragginPosX, (int) top, (int)Width, (int)Height, 0);
                    break;
                }
            }

            return IntPtr.Zero;
        }

        private static void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
