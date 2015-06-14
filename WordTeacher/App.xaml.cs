using System.Windows;
using WordTeacher.Extensions;
using WordTeacher.Utilities;
using WordTeacher.ViewModels;

namespace WordTeacher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SettingsUtility.CheckSettingsFolder();
            var translationItems = SettingsUtility.Load();
            
            var settingsViewModel = Current.Resources["SettingsViewModel"] as ITranslationsLoadable;
            if (settingsViewModel != null)
            {
                settingsViewModel.ReloadSettings(translationItems);
            }

            var mainViewModel = Current.Resources["MainViewModel"] as ITranslationsLoadable;
            if (mainViewModel != null)
            {
                mainViewModel.ReloadSettings(translationItems.Clone());
            }
        }
    }
}
