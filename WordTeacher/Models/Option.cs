using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WordTeacher.Models
{
    public class Option: INotifyPropertyChanged
    {
        private bool _isChecked;
        private Brush _color;

        public Option(string name) 
            : this(name, false)
        {
        }

        public Option(string name, bool isChecked)
            : this(name, isChecked, Brushes.Transparent)
        {
        }

        public Option(string name, bool isChecked, Brush color)
        {
            Name = name;
            IsChecked = isChecked;
            Color = color;
        }

        public string Name { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");

                var handler = IsCheckedChanged;
                if (handler != null)
                    handler.Invoke(this, new EventArgs());
            }
        }

        public Brush Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged("Color");
            }
        }


        public event EventHandler IsCheckedChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
