namespace Colonies
{
    using System.Collections.Generic;
    using System.Drawing;

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

            var ecosystem = new Ecosystem(habitats, new Dictionary<Organism, Coordinates>());
            var ecosystemViewModel = new EcosystemViewModel(ecosystem, habitatViewModels, eventaggregator);

            this.InitialiseTerrain(ecosystem);
            this.InitialiseOrganisms(ecosystem, eventaggregator);

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
                    //Terrain terrain;
                    //switch (x)
                    //{
                    //    case 0:
                    //        terrain = Terrain.Earth;
                    //        break;
                    //    case 1:
                    //        terrain = Terrain.Grass;
                    //        break;
                    //    case 2:
                    //        terrain = Terrain.Water;
                    //        break;
                    //    case 3:
                    //        terrain = Terrain.Fire;
                    //        break;
                    //    default:
                    //        terrain = Terrain.Unknown;
                    //        break;
                    //}

                    ecosystem.SetTerrain(new Coordinates(x, y), Terrain.Earth);
                }
            }
        }

        private void InitialiseOrganisms(Ecosystem ecosystem, EventAggregator eventaggregator)
        {
            // place some organisms in the ecosystem
            ecosystem.AddOrganism(new Organism("Waffle", Color.White), new Coordinates(0, 0));
            ecosystem.AddOrganism(new Organism("Wilber", Color.Black), new Coordinates(1, 1));
            ecosystem.AddOrganism(new Organism("Lotty", Color.Lime), new Coordinates(0, 4));
            ecosystem.AddOrganism(new Organism("Dr. Louise", Color.Orange), new Coordinates(4, 2));
        }
    }
}
