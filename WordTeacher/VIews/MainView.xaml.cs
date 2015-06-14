using System;
using System.Windows;
using WordTeacher.ViewModels;

namespace WordTeacher.VIews
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TimelineOnCompleted(object sender, EventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel != null)
                mainViewModel.IsHidden = !mainViewModel.IsHidden;
        }
    }
}
