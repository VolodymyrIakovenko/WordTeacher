using System.Windows;
using WordTeacher.ViewModels;

namespace WordTeacher.Views
{
    /// <summary>
    /// Interaction logic for TestView.xaml
    /// </summary>
    public partial class TestView : Window
    {
        public TestView()
        {
            InitializeComponent();
        }

        private void TestViewOnLoaded(object sender, RoutedEventArgs e)
        {
            var testViewModel = DataContext as TestViewModel;
            if (testViewModel != null)
                testViewModel.Opened();
        }
    }
}
