using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using WordTeacher.ViewModels;

namespace WordTeacher.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();

            if (DataContext is ICloseable)
            {
                (DataContext as ICloseable).RequestClose += (sender, args) => Close();
            }
        }

        private void DataGridOnRowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null || e.EditAction != DataGridEditAction.Commit)
                return;
           
            var view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource) as ListCollectionView;
            if (view != null && (view.IsAddingNew || view.IsEditingItem))
            {
                this.Dispatcher.BeginInvoke(new DispatcherOperationCallback(param =>
                {
                    var settingsViewModel = DataContext as SettingsViewModel;
                    if (settingsViewModel != null)
                        settingsViewModel.AreUnsavedChanges = true;
                    return null;
                }), DispatcherPriority.Background, new object[] { null });
            }
        }

        private void SettingsViewOnClosing(object sender, CancelEventArgs e)
        {
            var settingsViewModel = DataContext as SettingsViewModel;
            e.Cancel = settingsViewModel != null && !settingsViewModel.ExitSettings();
        }
    }
}
