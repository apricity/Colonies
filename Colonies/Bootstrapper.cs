namespace Colonies
{
    using System.Collections.Generic;

    using Colonies.Models;
    using Colonies.ViewModels;
    using Colonies.Views;

    using Microsoft.Practices.Prism.Events;

    public class Bootstrapper
    {
        public Bootstrapper()
        {
            
        }

        public void Run()
        {
            var eventaggregator = new EventAggregator();

            /* generate model tree */
            var habitats = new List<List<Habitat>>();
            for (var x = 0; x < 5; x++) // TODO: not hardcode
            {
                habitats.Add(new List<Habitat>());
                for (var y = 0; y < 5; y++)
                {
                    // initially set each habitat to have an unknown environment and no organism
                    var environment = new Environment(Terrain.Unknown);
                    var habitat = new Habitat(environment, null);
                    habitats[x].Add(habitat);
                }
            }

            var ecosystem = new Ecosystem(habitats);
            var mainWindow = new MainWindow(ecosystem);


            /* generate viewmodel tree */

            var habitatViewModels = new List<List<HabitatViewModel>>();
            for (var x = 0; x < 5; x++) // TODO: not hardcode
            {
                habitatViewModels.Add(new List<HabitatViewModel>());
                for (var y = 0; y < 5; y++)
                {
                    var environmentViewModel = new EnvironmentViewModel(habitats[x][y].Environment, eventaggregator);
                    var habitatViewModel = new HabitatViewModel(habitats[x][y], environmentViewModel, eventaggregator);
                    habitatViewModels[x].Add(habitatViewModel);
                }
            }

            var ecosystemViewModel = new EcosystemViewModel(ecosystem, habitatViewModels, eventaggregator);


            // create a view model for the main window
            // this will create the entire view-model tree and the underlying model
            var mainWindowViewModel = new MainWindowViewModel(mainWindow, ecosystemViewModel, eventaggregator);

            // create the view to display to the user
            // its data context is the view-model that has just been instantiated
            var mainWindowView = new MainWindowView { DataContext = mainWindowViewModel };

            // now that the view model tree and the underlying model are ready
            // display the window to the user!
            mainWindowView.Show();

            mainWindowViewModel.StartEcosystem();
        }
    }
}
