using System.Windows;
using WordTeacher.ViewModels;

namespace WordTeacher.Views
{
    /// <summary>
    /// Interaction logic for OptionsTestView.xaml
    /// </summary>
    public partial class OptionsTestView : Window
    {
        public OptionsTestView()
        {
            InitializeComponent();

            if (DataContext is ICloseable)
            {
                (DataContext as ICloseable).RequestClose += (sender, args) => Close();
            }
        }

        private void OptionsTestViewOnLoaded(object sender, RoutedEventArgs e)
        {
            var optionsTestViewModel = DataContext as OptionsTestViewModel;
            if (optionsTestViewModel != null)
                optionsTestViewModel.Opened();
        }
    }
}
