namespace Colonies
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;

    using Colonies.Annotations;

    public sealed class EcosystemViewModel : INotifyPropertyChanged
    {
        private readonly Timer modelTimer;

        // TODO: remove these, quick test variables
        private bool secondOrganismAdded = false;
        private bool leftToRightMovementDirection = true;

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

        private IEnumerable<Niche> ListOfOccupiedNiches
        {
            get
            {
                var result = new List<Niche>();

                for (int x = 0; x < this.Ecosystem.Width; x++)
                {
                    for (int y = 0; y < this.Ecosystem.Height; y++)
                    {
                        var niche = this.Ecosystem.GetNiche(x, y);
                        if (niche.Organism != null) // TODO: niche should have ContainsOrganism
                        {
                            result.Add(niche);
                        }
                    }
                }

                return result;
            }
        }

        public EcosystemViewModel()
        {
            this.InitialiseModel();

            TimerCallback modelTimerCallback = this.UpdateModel;
            this.modelTimer = new Timer(modelTimerCallback, null, 2000, Properties.Settings.Default.UpdateFrequencyInMs);
        }

        private void InitialiseModel()
        {
            this.Ecosystem = new Ecosystem();

            var random = new Random();

            for (var column = 0; column < Properties.Settings.Default.BoardWidth; column++)
            {
                var boardColumn = new List<Niche>();
                for (var row = 0; row < Properties.Settings.Default.BoardHeight; row++)
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

                    this.Ecosystem.SetNiche(column, row, new Niche(new Habitat(column, row, terrain), null));
                }
            }

            var organism = new Organism("testOrganism");
            var oldNiche = this.Ecosystem.GetNiche(1, 1);
            this.Ecosystem.SetNiche(1, 1, new Niche(oldNiche.Habitat, organism));;
        }

        private void UpdateModel(object state)
        {
            // TODO: don't actually do anything this stupid...
            // TODO: how should I handle updating the model?

            foreach (var boardElement in this.ListOfOccupiedNiches)
            {
                // temporarily store the Habitat Organism
                var habitat = boardElement.Habitat;
                var organism = boardElement.Organism;
                
                // remove Organism from this Habitat
                // and move it to a different Habitat
                // boardElement = null;

                if (this.leftToRightMovementDirection)
                {
                    if (habitat.X + 1 >= this.Ecosystem.Width)
                    {
                        this.leftToRightMovementDirection = false;

                        var targetHabitat = this.Ecosystem.GetNiche(habitat.X - 1, habitat.Y).Habitat;
                        this.Ecosystem.SetNiche(habitat.X - 1, habitat.Y, new Niche(targetHabitat, organism));

                    }
                    else
                    {
                        var targetHabitat = this.Ecosystem.GetNiche(habitat.X + 1, habitat.Y).Habitat;
                        this.Ecosystem.SetNiche(habitat.X + 1, habitat.Y, new Niche(targetHabitat, organism));
                    }
                }
                else
                {
                    if (habitat.X - 1 < 0)
                    {
                        this.leftToRightMovementDirection = true;

                        var targetHabitat = this.Ecosystem.GetNiche(habitat.X + 1, habitat.Y).Habitat;
                        this.Ecosystem.SetNiche(habitat.X + 1, habitat.Y, new Niche(targetHabitat, organism));
                    }
                    else
                    {
                        var targetHabitat = this.Ecosystem.GetNiche(habitat.X - 1, habitat.Y).Habitat;
                        this.Ecosystem.SetNiche(habitat.X - 1, habitat.Y, new Niche(targetHabitat, organism));
                    }
                }

                
            }

            if (!this.secondOrganismAdded)
            {
                var targetHabitat = this.Ecosystem.GetNiche(0, 0).Habitat;
                this.Ecosystem.SetNiche(0, 0, new Niche(targetHabitat, new Organism("anotherOrganism")));
                this.secondOrganismAdded = true;
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
