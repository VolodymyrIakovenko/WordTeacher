using System.Windows;
using WordTeacher.ViewModels;

namespace WordTeacher.Views
{
    /// <summary>
    /// Interaction logic for InputTestView.xaml
    /// </summary>
    public partial class InputTestView : Window
    {
        public InputTestView()
        {
            InitializeComponent();

            if (DataContext is ICloseable)
            {
                (DataContext as ICloseable).RequestClose += (sender, args) => Close();
            }
        }

        private void InputTestViewOnLoaded(object sender, RoutedEventArgs e)
        {
            var inputTestViewModel = DataContext as InputTestViewModel;
            if (inputTestViewModel != null)
                inputTestViewModel.Opened();
        }
    }
}
