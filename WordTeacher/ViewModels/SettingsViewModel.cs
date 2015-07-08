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
        private string _currentCategorySetting;
        private string _newCategoryName;
        private TranslationItem _selectedTranslationItem;
        private TranslationItem _shownTranslationItem;
        private Category _currentCategory;

        private ObservableCollection<string> _categoryNames = new ObservableCollection<string>(); 
        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>();
        private List<Category> _categories = new List<Category>();
        private List<Category> _savedCategories = new List<Category>();

        private ICommand _addCategoryCommand;
        private ICommand _deleteCategoryCommand;
        private ICommand _renameCategoryCommand;
        private ICommand _addWordCommand;
        private ICommand _deleteWordCommand;
        private ICommand _chooseWordCommand;
        private ICommand _saveCommand;
        private ICommand _exitCommand;

        public SettingsViewModel()
        {
            Opened();
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

        public string CurrentCategorySetting
        {
            get { return _currentCategorySetting; }
            set
            {
                _currentCategorySetting = value;
                CurrentCategory = GetCurrentFromCategories(CurrentCategorySetting);
                OnPropertyChanged("CurrentCategorySetting");
                UpdateIfAnyNewSettings();
            }
        }

        public string NewCategoryName
        {
            get { return _newCategoryName; }
            set
            {
                _newCategoryName = value;
                OnPropertyChanged("NewCategoryName");
                OnPropertyChanged("IsChangeCategoryEnabled");
            }
        }

        public bool IsChangeCategoryEnabled
        {
            get { return NewCategoryName != null && !NewCategoryName.Equals(string.Empty) && !NewCategoryName.Equals(CurrentCategorySetting); }
        }

        public bool IsDeleteCategoryEnabled
        {
            get { return CategoryNames.Count > 1; }
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

        public ObservableCollection<string> CategoryNames
        {
            get { return _categoryNames; }
            set
            {
                _categoryNames = value;
                
                UpdateCategories();
                
                OnPropertyChanged("CategoryNames");
                OnPropertyChanged("IsDeleteCategoryEnabled");
            }
        }

        public List<Category> SavedCategories
        {
            get { return _savedCategories; }
            set
            {
                _savedCategories = value;
                var currentSavedCategory = GetCurrentFromSavedCategories(CurrentCategorySetting);
                var handler = WordsSaved;
                if (handler != null && currentSavedCategory != null)
                {
                    handler.Invoke(this, currentSavedCategory);
                }
            }
        }

        public List<Category> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
            }
        }

        public Category CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                if (value == null)
                    return;

                _currentCategory = value;
                TranslationItems = new ObservableCollection<TranslationItem>(CurrentCategory.TranslationItems);
            }
        }

        public ObservableCollection<TranslationItem> TranslationItems
        {
            get { return _translationItems; }
            set
            {
                _translationItems = value;
                TranslationItemsChanged();
                OnPropertyChanged("TranslationItems");
            }
        }

        public ICommand AddCategoryCommand
        {
            get { return _addCategoryCommand ?? (_addCategoryCommand = new CommandHandler(AddCategory, true)); }
        }

        public ICommand DeleteCategoryCommand
        {
            get { return _deleteCategoryCommand ?? (_deleteCategoryCommand = new CommandHandler(DeleteCategory, true)); }
        }

        public ICommand RenameCategoryCommand
        {
            get { return _renameCategoryCommand ?? (_renameCategoryCommand = new CommandHandler(RenameCategory, true)); }
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
        public event EventHandler<Category> WordsSaved;

        /// <summary>
        /// Called when the view was loaded.
        /// </summary>
        public void Opened()
        {
            // Assign loaded settings.
            RandomSetting = Settings.Default.NextRandom;
            AutoStartSetting = Settings.Default.AutoStart;
            AutoChangeSetting = Settings.Default.AutoChange;
            ChangeInMinutesSetting = Settings.Default.ChangeInMinutes;
            CurrentCategorySetting = Settings.Default.CurrentCategory;

            // Load categories from files.
            var files = SettingFilesUtility.GetAllXmlFiles();
            CategoryNames = new ObservableCollection<string>(files);

            // Update the flag that defines if there are any unsaved settings.
            UpdateIfAnyNewSettings();
        }

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

        public void TranslationItemsChanged()
        {
            CurrentCategory.TranslationItems = new List<TranslationItem>(TranslationItems);
            Categories.Find(x => x.Name.Equals(CurrentCategory.Name)).TranslationItems = CurrentCategory.TranslationItems;
            UpdateIfAnyNewSettings();
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

        private void AddCategory()
        {
            CategoryNames.Add(NewCategoryName);
            OnPropertyChanged("CategoryNames");

            Categories.Add(new Category(NewCategoryName, new List<TranslationItem>()));
            CurrentCategorySetting = NewCategoryName;

            NewCategoryName = string.Empty;
            OnPropertyChanged("IsDeleteCategoryEnabled");
        }

        private void DeleteCategory()
        {
            if (Categories.Count <= 1)
                return;

            var categoryToRemove = CurrentCategorySetting;

            CategoryNames.Remove(categoryToRemove);
            OnPropertyChanged("CategoryNames");

            Categories.Remove(Categories.Find(x => x.Name.Equals(categoryToRemove)));
            CurrentCategorySetting = Categories[0].Name;

            OnPropertyChanged("IsDeleteCategoryEnabled");
        }

        private void RenameCategory()
        {
            var categoryToRename = CurrentCategorySetting;

            var categoryNameIndexToRename = CategoryNames.IndexOf(categoryToRename);
            CategoryNames[categoryNameIndexToRename] = NewCategoryName;
            OnPropertyChanged("CategoryNames");

            var categoryIndexToRename = Categories.FindIndex(c => c.Name.Equals(categoryToRename));
            Categories[categoryIndexToRename].Name = NewCategoryName;
            CurrentCategorySetting = CategoryNames[categoryNameIndexToRename];

            NewCategoryName = string.Empty;
        }

        private void AddWord()
        {
            TranslationItems.Add(new TranslationItem());
            TranslationItemsChanged();
            SelectedTranslationItem = TranslationItems[TranslationItems.Count - 1];
            UpdateIfAnyNewSettings();
        }

        private void DeleteWord()
        {
            if (Categories == null || SelectedTranslationItem == null)
                return;

            if (!TranslationItems.Remove(SelectedTranslationItem))
                return;
    
            TranslationItemsChanged();
            SelectedTranslationItem = null;
            UpdateIfAnyNewSettings();
        }

        private void ChooseWord()
        {
            var handler = SelectedWordUpdated;
            if (handler != null)
                handler.Invoke(this, SelectedTranslationItem);
        }

        private void UpdateCategories()
        {
            var loadedCategories = _categoryNames.Select(SettingFilesUtility.Load).ToList();
                Categories = new List<Category>(loadedCategories.Clone());
                SavedCategories = new List<Category>(loadedCategories.Clone());
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
                   CurrentCategorySetting != Settings.Default.CurrentCategory ||
                   !Categories.SequenceEqual(SavedCategories);
        }

        /// <summary>
        /// Rollbacks all settings to saved ones.
        /// </summary>
        private void RollbackSettings()
        {
            Categories = new List<Category>(SavedCategories.Clone());
            RandomSetting = Settings.Default.NextRandom;
            AutoStartSetting = Settings.Default.AutoStart;
            AutoChangeSetting = Settings.Default.AutoChange;
            ChangeInMinutesSetting = Settings.Default.ChangeInMinutes;
            CurrentCategorySetting = Settings.Default.CurrentCategory;
            
            AreUnsavedChanges = false;
        }

        /// <summary>
        /// Save all settings.
        /// </summary>
        private void SaveSettings()
        {
            // Save and add categories.
            foreach (var categoryToSave in Categories)
                SettingFilesUtility.Save(categoryToSave);
 
            // Delete categories.
            foreach (var categoryNameToDelete in SavedCategories.Select(x => x.Name).Except(Categories.Select(x => x.Name)))
                SettingFilesUtility.Delete(categoryNameToDelete);

            Settings.Default.NextRandom = RandomSetting;
            Settings.Default.AutoStart = AutoStartSetting;
            Settings.Default.AutoChange = AutoChangeSetting;
            Settings.Default.ChangeInMinutes = ChangeInMinutesSetting;
            Settings.Default.CurrentCategory = CurrentCategorySetting;
            Settings.Default.Save();

            // Updated categories list and saved categories.
            SavedCategories = new List<Category>(Categories.Clone());
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

        private Category GetCurrentFromCategories(string file)
        {
            return Categories.FirstOrDefault(x => x.Name.Equals(file));
        }

        private Category GetCurrentFromSavedCategories(string file)
        {
            return SavedCategories.FirstOrDefault(x => x.Name.Equals(file));
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
