namespace Colonies
{
    using System.Collections.Generic;
    using System.Drawing;

    using Colonies.Events;
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

            // automatically start the ecosystem
            mainViewModel.StartEcosystem();
        }

        private MainViewModel BuildMainDataContext()
        {
            // the event aggregator is going to be used by view models to inform of changes
            var eventaggregator = new EventAggregator();

            var habitats = new List<List<Habitat>>();
            var habitatViewModels = new List<List<HabitatViewModel>>();

            for (var x = 0; x < Properties.Settings.Default.EcosystemWidth; x++)
            {
                habitats.Add(new List<Habitat>());
                habitatViewModels.Add(new List<HabitatViewModel>());

                for (var y = 0; y < Properties.Settings.Default.EcosystemHeight; y++)
                {
                    // initially set each habitat to have an unknown environment and no organism
                    var environment = new Environment(Terrain.Unknown);
                    var environmentViewModel = new EnvironmentViewModel(environment, eventaggregator);

                    var habitat = new Habitat(environment, null);
                    var habitatViewModel = new HabitatViewModel(habitat, environmentViewModel, eventaggregator);

                    habitats[x].Add(habitat);
                    habitatViewModels[x].Add(habitatViewModel);
                }
            }

            var ecosystem = new Ecosystem(habitats);
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
                    Terrain terrain;
                    switch (x)
                    {
                        case 0:
                            terrain = Terrain.Earth;
                            break;
                        case 1:
                            terrain = Terrain.Grass;
                            break;
                        case 2:
                            terrain = Terrain.Water;
                            break;
                        case 3:
                            terrain = Terrain.Fire;
                            break;
                        default:
                            terrain = Terrain.Unknown;
                            break;
                    }

                    ecosystem.Habitats[x][y].Environment.Terrain = terrain;
                }
            }
        }

        private void InitialiseOrganisms(Ecosystem ecosystem, EventAggregator eventaggregator)
        {
            // place some organisms in the ecosystem
            ecosystem.Habitats[0][0].Organism = new Organism("Waffle", Color.White);
            ecosystem.Habitats[1][1].Organism = new Organism("Wacton", Color.Black);
            ecosystem.Habitats[0][4].Organism = new Organism("Lotty", Color.Lime);
            ecosystem.Habitats[4][2].Organism = new Organism("Louise", Color.Orange);

            // TODO: should this event be published by the model itself
            eventaggregator.GetEvent<OrganismsUpdatedEvent>().Publish(null);
        }
    }
}
