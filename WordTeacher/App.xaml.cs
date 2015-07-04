using System;
using System.Windows;
using Microsoft.Win32;
using WordTeacher.Properties;
using WordTeacher.Utilities;

namespace WordTeacher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string ApplicationName = "WordTeacher";

        // The path to the key where Windows looks for startup applications
        private readonly RegistryKey _registryKeyApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        private bool _autoStartSetting;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SettingFilesUtility.CheckSettingsFolder();

            UpdateAutoStart(Settings.Default.AutoStart);
            Settings.Default.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals("AutoStart") && Settings.Default.AutoStart != _autoStartSetting)
                    UpdateAutoStart(Settings.Default.AutoStart);
            };
        }

        private void UpdateAutoStart(bool newValue)
        {
            if (newValue)
            {
                // Add the value in the registry so that the application runs at startup
                _registryKeyApp.SetValue(ApplicationName, "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"" );
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                _registryKeyApp.DeleteValue(ApplicationName, false);
            }
            _autoStartSetting = newValue;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var mainViewModel = Current.Resources["MainViewModel"] as IDisposable;
            if (mainViewModel != null)
            {
                mainViewModel.Dispose();
            }
        }
    }
}