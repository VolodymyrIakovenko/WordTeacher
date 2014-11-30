using System.Collections.ObjectModel;
using System.ComponentModel;
using WordTeacher.Models;
using WordTeacher.Utilities;

namespace WordTeacher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private double _positionX;
        private double _positionY;
        private ObservableCollection<ContextAction> _contextActions = new ObservableCollection<ContextAction>();

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
        /// List of the context menu items.
        /// </summary>
        public ObservableCollection<ContextAction> ContextActions
        {
            get { return _contextActions; }
            set
            {
                _contextActions = value;
                OnPropertyChanged("ContextActions");
            }
        }

        public MainViewModel()
        {
            ArrangeWindowPosition();
            CreateContextItems();
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

        /// <summary>
        /// Add context menu items.
        /// </summary>
        private void CreateContextItems()
        {
            ContextActions = new ObservableCollection<ContextAction>
            {
                new ContextAction("Settings", null),
                new ContextAction("Exit", null)
            };

            OnPropertyChanged("ContextActions");
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
