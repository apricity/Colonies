namespace Wacton.Colonies
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Linq;

    using Microsoft.Practices.Prism.Events;

    using Wacton.Colonies.Models;
    using Wacton.Colonies.ViewModels;
    using Wacton.Colonies.Views;

    public class Bootstrapper
    {
        public Bootstrapper()
        {
            
        }

        public void Run()
        {
            // create the view to display to the user
            // the data context is the view model tree that contains the model
            var mainViewModel = this.BuildMainDataContext();
            var mainView = new MainView { DataContext = mainViewModel };

            // display the window to the user!
            mainView.Show();
        }

        private MainViewModel BuildMainDataContext()
        {
            var width = Properties.Settings.Default.EcosystemWidth;
            var height = Properties.Settings.Default.EcosystemHeight;

            // the event aggregator might be used by view models to inform of changes
            var eventaggregator = new EventAggregator();

            var habitats = new Habitat[width, height];
            var habitatViewModels = new List<List<HabitatViewModel>>();
            
            for (var x = 0; x < width; x++)
            {
                habitatViewModels.Add(new List<HabitatViewModel>());

                for (var y = 0; y < height; y++)
                {
                    // initially set each habitat to have an unknown environment and no organism
                    var environment = new Environment(Terrain.Unknown, false);
                    var environmentViewModel = new EnvironmentViewModel(environment, eventaggregator);

                    var organismViewModel = new OrganismViewModel(null, eventaggregator);

                    var habitat = new Habitat(environment, null);
                    var habitatViewModel = new HabitatViewModel(habitat, environmentViewModel, organismViewModel, eventaggregator);

                    habitats[x, y] = habitat;
                    habitatViewModels[x].Add(habitatViewModel);
                }
            }

            var ecosystem = new Ecosystem(habitats, new Dictionary<Organism, Habitat>());
            var ecosystemViewModel = new EcosystemViewModel(ecosystem, habitatViewModels, eventaggregator);

            this.InitialiseTerrain(ecosystem);
            var initialOrganismCoordinates = this.InitialiseOrganisms(ecosystem);

            // TODO: do this in InitialiseOrganisms, all getting a bit too messy here...
            // TODO: boshed together so the organisms are visible before ecosystem is switched on the first time
            var summaryOrganismViewModels = new List<OrganismViewModel>();            
            foreach (var organismCoordinate in initialOrganismCoordinates)
            {
                var organism = ecosystem.Habitats[organismCoordinate.X, organismCoordinate.Y].Organism;

                // hook organism model into the ecosystem
                habitatViewModels[organismCoordinate.X][organismCoordinate.Y].OrganismViewModel.AssignModel(organism);
                habitatViewModels[organismCoordinate.X][organismCoordinate.Y].OrganismViewModel.Refresh();

                // hook organism model into the organism summary
                summaryOrganismViewModels.Add(new OrganismViewModel(organism, eventaggregator));
            }

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    habitatViewModels[x][y].EnvironmentViewModel.Refresh();
                }
            }

            // TODO: setting up the summary view after organisms initialised... how to handle this nicely
            var organismSummaryViewModel = new OrganismSummaryViewModel(ecosystem, summaryOrganismViewModels, eventaggregator);

            var main = new Main(ecosystem);
            var mainViewModel = new MainViewModel(main, ecosystemViewModel, organismSummaryViewModel, eventaggregator);

            return mainViewModel;
        }

        private void InitialiseTerrain(Ecosystem ecosystem)
        {
            var nutrientCoordinates = new List<Coordinates> { new Coordinates(0, 0) };
            foreach (var coordinates in nutrientCoordinates)
            {
                ecosystem.Habitats[coordinates.X, coordinates.Y].HasNutrient = true;
            }

            // custom obstructed habitats (will make a square shapen with an entrance - a pen?)
            var obstructedCoordinates = new List<Coordinates>
                                            {
                                                new Coordinates(1, 1),
                                                new Coordinates(1, 2),
                                                new Coordinates(1, 3),
                                                new Coordinates(1, 4),
                                                new Coordinates(1, 5),
                                                new Coordinates(1, 6),
                                                new Coordinates(1, 7),
                                                new Coordinates(1, 8),
                                                new Coordinates(2, 1),
                                                new Coordinates(3, 1),
                                                new Coordinates(4, 1),
                                                new Coordinates(5, 1),
                                                new Coordinates(6, 1),
                                                new Coordinates(7, 1),
                                                new Coordinates(2, 8),
                                                new Coordinates(3, 8),
                                                new Coordinates(4, 8),
                                                new Coordinates(5, 8),
                                                new Coordinates(6, 8),
                                                new Coordinates(7, 8),
                                                new Coordinates(8, 1),
                                                new Coordinates(8, 2),
                                                new Coordinates(8, 3),
                                                new Coordinates(8, 6),
                                                new Coordinates(8, 7),
                                                new Coordinates(8, 8)
                                            };

            foreach (var coordinates in obstructedCoordinates)
            {
                ecosystem.Habitats[coordinates.X, coordinates.Y].SetObstructed(true);
            }
        }

        private IEnumerable<Coordinates> InitialiseOrganisms(Ecosystem ecosystem)
        {
            var waffleCoords = new Coordinates(2, 2);
            var wilberCoords = new Coordinates(2, 7);
            var lottyCoords = new Coordinates(7, 2);
            var louiseCoords = new Coordinates(7, 7);

            // place some organisms in the ecosystem
            ecosystem.AddOrganism(new Organism("Waffle", Colors.White, true), waffleCoords);
            ecosystem.AddOrganism(new Organism("Wilber", Colors.Black, true), wilberCoords);
            ecosystem.AddOrganism(new Organism("Lotty", Colors.Lime, true), lottyCoords);
            ecosystem.AddOrganism(new Organism("Dr. Louise", Colors.Orange, true), louiseCoords);

            return new List<Coordinates> { waffleCoords, wilberCoords, lottyCoords, louiseCoords };
        }
    }
}
