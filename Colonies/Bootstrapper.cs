namespace Wacton.Colonies
{
    using System.Collections.Generic;
    using System.Drawing;

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

            // TODO: do this in InitialiseOrganisms
            // boshed together so the organisms are visible before ecosystem is switched on the first time
            foreach (var organismCoordinate in initialOrganismCoordinates)
            {
                habitatViewModels[organismCoordinate.X][organismCoordinate.Y].RefreshOrganismViewModel();
            }

            var main = new Main(ecosystem);
            var mainViewModel = new MainViewModel(main, ecosystemViewModel, eventaggregator);

            return mainViewModel;
        }

        private void InitialiseTerrain(Ecosystem ecosystem)
        {
            // apply a terrain for every habitat
            for (var x = 0; x < ecosystem.Width; x++)
            {
                for (var y = 0; y < ecosystem.Height; y++)
                {
                    //ecosystem.SetTerrain(new Coordinates(x, y), Terrain.Something);
                }
            }

            // custom obstructed habitats (will make a square shapen with an entrance - a pen?)
            ecosystem.Habitats[1, 1].SetObstructed(true);
            ecosystem.Habitats[1, 2].SetObstructed(true);
            ecosystem.Habitats[1, 3].SetObstructed(true);
            ecosystem.Habitats[1, 4].SetObstructed(true);
            ecosystem.Habitats[1, 5].SetObstructed(true);
            ecosystem.Habitats[1, 6].SetObstructed(true);
            ecosystem.Habitats[1, 7].SetObstructed(true);
            ecosystem.Habitats[1, 8].SetObstructed(true);

            ecosystem.Habitats[2, 1].SetObstructed(true);
            ecosystem.Habitats[3, 1].SetObstructed(true);
            ecosystem.Habitats[4, 1].SetObstructed(true);
            ecosystem.Habitats[5, 1].SetObstructed(true);
            ecosystem.Habitats[6, 1].SetObstructed(true);
            ecosystem.Habitats[7, 1].SetObstructed(true);

            ecosystem.Habitats[2, 8].SetObstructed(true);
            ecosystem.Habitats[3, 8].SetObstructed(true);
            ecosystem.Habitats[4, 8].SetObstructed(true);
            ecosystem.Habitats[5, 8].SetObstructed(true);
            ecosystem.Habitats[6, 8].SetObstructed(true);
            ecosystem.Habitats[7, 8].SetObstructed(true);

            ecosystem.Habitats[8, 1].SetObstructed(true);
            ecosystem.Habitats[8, 2].SetObstructed(true);
            ecosystem.Habitats[8, 3].SetObstructed(true);
            ecosystem.Habitats[8, 6].SetObstructed(true);
            ecosystem.Habitats[8, 7].SetObstructed(true);
            ecosystem.Habitats[8, 8].SetObstructed(true);
        }

        private IEnumerable<Coordinates> InitialiseOrganisms(Ecosystem ecosystem)
        {
            var waffleCoords = new Coordinates(2, 2);
            var wilberCoords = new Coordinates(2, 7);
            var lottyCoords = new Coordinates(7, 2);
            var louiseCoords = new Coordinates(7, 7);

            // place some organisms in the ecosystem
            ecosystem.AddOrganism(new Organism("Waffle", Color.White, true), waffleCoords);
            ecosystem.AddOrganism(new Organism("Wilber", Color.Black, true), wilberCoords);
            ecosystem.AddOrganism(new Organism("Lotty", Color.Lime, true), lottyCoords);
            ecosystem.AddOrganism(new Organism("Dr. Louise", Color.Orange, true), louiseCoords);

            return new List<Coordinates> { waffleCoords, wilberCoords, lottyCoords, louiseCoords };
        }
    }
}
