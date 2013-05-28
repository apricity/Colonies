namespace Colonies
{
    using System.Collections.Generic;
    using System.Drawing;

    using Colonies.Logic;
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
                    var environment = new Environment(Terrain.Unknown);
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
                //ecosystem.SetTerrain(new Coordinates(x, 0), Terrain.Impassable);
                for (var y = 0; y < ecosystem.Height; y++)
                {
                    //ecosystem.SetTerrain(new Coordinates(0, y), Terrain.Impassable);
                }
            }

            ecosystem.SetTerrain(new Coordinates(1, 1), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(1, 2), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(2, 1), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(2, 2), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(5, 3), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(5, 4), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(6, 3), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(6, 4), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(14, 5), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(14, 6), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(15, 5), Terrain.Impassable);
            ecosystem.SetTerrain(new Coordinates(15, 6), Terrain.Impassable);
            // testing drawing of pheromones
            //ecosystem.IncreasePheromoneLevel(new Coordinates(5, 0), 1);
            //ecosystem.IncreasePheromoneLevel(new Coordinates(5, 1), 0.75);
            //ecosystem.IncreasePheromoneLevel(new Coordinates(5, 2), 0.5);
            //ecosystem.IncreasePheromoneLevel(new Coordinates(5, 3), 0.25);
            //ecosystem.IncreasePheromoneLevel(new Coordinates(5, 4), 0);
        }

        private IEnumerable<Coordinates> InitialiseOrganisms(Ecosystem ecosystem)
        {
            var waffleCoords = new Coordinates(4, 4);
            var wilberCoords = new Coordinates(0, 0);
            var lottyCoords = new Coordinates(1, 4);
            var louiseCoords = new Coordinates(4, 2);

            // place some organisms in the ecosystem
            ecosystem.AddOrganism(new Organism("Waffle", Color.White, true), waffleCoords);
            ecosystem.AddOrganism(new Organism("Wilber", Color.Black, true), wilberCoords);
            ecosystem.AddOrganism(new Organism("Lotty", Color.Lime, true), lottyCoords);
            ecosystem.AddOrganism(new Organism("Dr. Louise", Color.Orange, true), louiseCoords);

            return new List<Coordinates> { waffleCoords, wilberCoords, lottyCoords, louiseCoords };
        }
    }
}
