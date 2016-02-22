namespace Wacton.Colonies
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    using Wacton.Tovarisch.Logging;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Colonies.Startup.Go();
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Default.Error(e.Exception, "Unhandled exception (dispatcher)");
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Default.Error((Exception)e.ExceptionObject, "Unhandled exception (domain)");
        }

        private static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.Default.Error(e.Exception, "Unhandled exception (unobserved task)");
        }
    }
}