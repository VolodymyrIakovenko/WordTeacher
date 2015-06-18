using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
                        settingsViewModel.UpdateIfAnyNewSettings();
                    return null;
                }), DispatcherPriority.Background, new object[] { null });
            }
        }

        private void SettingsViewOnClosing(object sender, CancelEventArgs e)
        {
            var settingsViewModel = DataContext as SettingsViewModel;
            e.Cancel = settingsViewModel != null && !settingsViewModel.ExitSettings();
        }

        private void TextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var newChar = e.Text[e.Text.Length - 1];
            if (!char.IsDigit(newChar))
            {
                e.Handled = true;
            }
        }

        private void TextBoxBaseOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null)
                return;

            // Textbox's value should not be empty and less than 1.
            if (textBox.Text.Equals(string.Empty) || textBox.Text.Equals("0"))
                textBox.Text = "1";

            // Remove zeros from the beginning of the textbox's value.
            textBox.Text = textBox.Text.TrimStart('0');

            var settingsViewModel = DataContext as SettingsViewModel;
            if (settingsViewModel != null)
                settingsViewModel.ChangeInMinutesSetting = Convert.ToInt32(textBox.Text);
            
        }
    }
}
