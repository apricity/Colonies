using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;
    using System.Threading;

    using Colonies.Annotations;

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
            var initialEcosystem = this.InitialiseEcosystem(
                Properties.Settings.Default.EcosystemHeight, Properties.Settings.Default.EcosystemWidth);

            // second: add terrain and organisms to the ecosystem
            this.RegenerateTerrain(initialEcosystem);
            this.RegenerateOrganisms(initialEcosystem);

            // third: generate a new ecosystem view-model with the ecosystem
            // (this will propogate downwards, generating the full view-model tree
            //  - EcosystemViewModel will create NicheViewModels from the Ecosystem that it's given, and so on)
            // this will fire off OnPropertyChanged events, causing the UI to update with the new ecosystem model
            this.EcosystemViewModel = new EcosystemViewModel(initialEcosystem);

            // start the ecosystem so that it continually updates itself
            this.EcosystemViewModel.StartEcosystem();
        }

        private Ecosystem InitialiseEcosystem(int height, int width)
        {
            // create a 2D array of niches, which will represent the ecosystem
            var niches = new List<List<Niche>>();
            for (var x = 0; x < width; x++)
            {
                niches.Add(new List<Niche>());
                for (var y = 0; y < width; y++)
                {
                    // initially set each niche to have an unknown habitat and no organism
                    var habitat = new Habitat(Terrain.Unknown);
                    var niche = new Niche(habitat, null);
                    niches[x].Add(niche);
                }
            }

            return new Ecosystem(niches);
        }

        private void RegenerateTerrain(Ecosystem ecosystem)
        {
            // apply a terrain for every niche
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

                    ecosystem.Niches[x][y].Habitat.Terrain = terrain;
                }
            }
        }

        private void RegenerateOrganisms(Ecosystem ecosystem)
        {
            // place some organisms in the ecosystem
            // nothing clever yet, just removing all organisms and adding one in a starting position
            for (var x = 0; x < ecosystem.Width; x++)
            {
                for (var y = 0; y < ecosystem.Height; y++)
                {
                    ecosystem.Niches[x][y].Organism = null;
                }
            }

            ecosystem.Niches[0][0].Organism = new Organism("Wacton");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
