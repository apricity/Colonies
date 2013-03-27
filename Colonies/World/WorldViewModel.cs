namespace Colonies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;

    using Colonies.Annotations;

    public sealed class WorldViewModel : INotifyPropertyChanged
    {
        private readonly Timer modelTimer;

        private Ecosystem ecosystem;
        public Ecosystem Ecosystem
        {
            get
            {
                return this.ecosystem;
            }
            set
            {
                this.ecosystem = value;
                this.OnPropertyChanged("Ecosystem");
            }
        }

        public WorldViewModel()
        {
            this.InitialiseModel();

            TimerCallback modelTimerCallback = this.UpdateModel;
            this.modelTimer = new Timer(modelTimerCallback, null, 2000, Properties.Settings.Default.UpdateFrequencyInMs);
        }

        private void InitialiseModel()
        {
            this.Ecosystem = new Ecosystem();

            var random = new Random();

            for (var column = 0; column < this.Ecosystem.Width; column++)
            {
                for (var row = 0; row < this.Ecosystem.Height; row++)
                {
                    Terrain terrain;

                    var randomNumber = random.Next(0, 4);
                    switch (column)
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

                    // TODO: Habitat should not know about its coordinates
                    this.Ecosystem.SetNiche(column, row, new Niche(new Habitat(terrain), null));
                }
            }

            this.Ecosystem.AddOrganism(1, 1, new Organism("testOrganism"));
        }

        private void UpdateModel(object state)
        {
            // TODO: don't actually do anything this stupid...
            // TODO: how should I handle updating the model?

            var organisms = this.Ecosystem.GetOrganisms();
            foreach (var organism in organisms)
            {
                this.Ecosystem.MoveOrganismToRandomAvailableNiche(organism);
            }
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
