namespace Colonies.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Colonies.Annotations;
    using Colonies.Models;

    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private EcosystemViewModel ecosystemViewModel;
        public EcosystemViewModel EcosystemViewModel
        {
            get
            {
                return this.ecosystemViewModel;
            }
            set
            {
                this.ecosystemViewModel = value;
                this.OnPropertyChanged("EcosystemViewModel");
            }
        }

        public MainWindowViewModel()
        {
            // first: build an initial underlying ecosystem model consisting of nothing
            var initialEcosystem = this.InitialiseBaseEcosystem(
                Properties.Settings.Default.EcosystemHeight, Properties.Settings.Default.EcosystemWidth);

            // second: add terrain and organisms to the ecosystem
            this.InitialiseTerrain(initialEcosystem);
            this.InitialiseOrganisms(initialEcosystem);

            // third: generate a new ecosystem view-model with the initial ecosystem
            // (this will propogate downwards, generating the full view-model tree
            //  - EcosystemViewModel will create HabitatViewModels from the Ecosystem that it's given, and so on)
            // this will fire off OnPropertyChanged events, causing the UI to update with the new ecosystem model
            this.EcosystemViewModel = new EcosystemViewModel(initialEcosystem);

            // start the ecosystem so that it continually updates itself
            this.EcosystemViewModel.StartEcosystem();
        }

        private Ecosystem InitialiseBaseEcosystem(int height, int width)
        {
            // create a 2D array of habitats, which will represent the ecosystem
            var habitats = new List<List<Habitat>>();
            for (var x = 0; x < width; x++)
            {
                habitats.Add(new List<Habitat>());
                for (var y = 0; y < width; y++)
                {
                    // initially set each habitat to have an unknown environment and no organism
                    var environment = new Environment(Terrain.Unknown);
                    var habitat = new Habitat(environment, null);
                    habitats[x].Add(habitat);
                }
            }

            return new Ecosystem(habitats);
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

        private void InitialiseOrganisms(Ecosystem ecosystem)
        {
            // place some organisms in the ecosystem
            // nothing clever yet, just removing all organisms and adding one in a starting position
            for (var x = 0; x < ecosystem.Width; x++)
            {
                for (var y = 0; y < ecosystem.Height; y++)
                {
                    ecosystem.Habitats[x][y].Organism = null;
                }
            }

            ecosystem.Habitats[0][0].Organism = new Organism("Wacton");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
