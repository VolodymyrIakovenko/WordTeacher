using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using WordTeacher.ViewModels;

namespace WordTeacher.VIews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _startPoint;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TimelineOnCompleted(object sender, EventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel != null)
            {
                mainViewModel.IsHidden = !mainViewModel.IsHidden;
            }
        }

        private void MainWindowOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MainWindowOnLoaded(object sender, RoutedEventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel == null)
                return;

            var helper = new WindowInteropHelper(this);
            mainViewModel.WindowLoaded(helper.Handle);
        }

        private void CollapseButtonOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = (Button)sender;
            _startPoint = e.GetPosition(button);
        }

        private void CollapseButtonOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var button = (Button)sender;
            var currentPoint = e.GetPosition(button);
            if (e.LeftButton == MouseButtonState.Pressed && button.IsMouseCaptured &&
                (Math.Abs(currentPoint.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(currentPoint.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Prevent click from firing.
                button.ReleaseMouseCapture();
                DragMove();
            }
        }
    }
}
