using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WordTeacher.Models;

namespace WordTeacher.ViewModels
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        private ObservableCollection<TranslationItem> _translationItems = new ObservableCollection<TranslationItem>(); 

        public SettingsViewModel(IEnumerable<TranslationItem> translationItems)
        {
            TranslationItems = new ObservableCollection<TranslationItem>(translationItems);
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
