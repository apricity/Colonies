using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Colonies
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += App.OnUnhandledException;
        }

        // TODO: replace xaml startupUri with something that composes the model and viewmodels?
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    var worldViewModel = new WorldViewModel();
        //    var worldView = new WorldView { DataContext = worldViewModel };

        //    var mainWindowView = new MainWindow();
        //    mainWindowView.Show();
        //}

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO: log unhandled exceptions
            throw new NotImplementedException();
        }
    }
}
