namespace Colonies
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // hook up unhandled exception handling
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        } 
    }
}
