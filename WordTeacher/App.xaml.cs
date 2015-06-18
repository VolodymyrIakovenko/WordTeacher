using System;
using System.Windows;

namespace WordTeacher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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