using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Colonies
{
    using System.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            // hook up unhandled exception handling
            AppDomain.CurrentDomain.UnhandledException += App.OnUnhandledException;

            // create a view model for the main window
            // this will create the entire view-model tree and the underlying model
            var mainWindowViewModel = new MainWindowViewModel();

            // create the view to display to the user
            // its data context is the view-model that has just been instantiated
            var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };

            // now that the view model tree and the underlying model are ready
            // display the window to the user!
            mainWindowView.Show();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        } 
    }
}
