using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colonies
{
    using System.ComponentModel;

    using Colonies.Annotations;

    public sealed class Ecosystem : INotifyPropertyChanged
    {
        private List<List<Niche>> niches;
        public List<List<Niche>> Niches
        {
            get
            {
                return this.niches;
            }
            set
            {
                this.niches = value;
                this.OnPropertyChanged("Niches");
            }
        }

        public int Height { get; private set; }
        public int Width { get; private set; }

        public Ecosystem()
        {
            this.Height = Properties.Settings.Default.BoardHeight;
            this.Width = Properties.Settings.Default.BoardWidth;

            var board = new List<List<Niche>>(this.Height);

            for (int i = 0; i < board.Capacity; i++)
            {
                board.Add(new List<Niche>(this.Width));

                for (int j = 0; j < board[i].Capacity; j++)
                {
                    board[i].Add(new Niche(null, null));
                }
            }

            this.niches = board;
        }

        public void SetNiche(int x, int y, Niche niche)
        {
            this.niches[x][y] = niche;
            this.OnPropertyChanged("Niches");
        }

        public Niche GetNiche(int x, int y)
        {
            return this.niches[x][y];
        }

        public bool ContainsOrganism(int x, int y)
        {
            return this.niches[x][y].Organism != null;
        }

        public void AddOrganism(int x, int y, Organism organism)
        {
            if (this.ContainsOrganism(x, y))
            {
                throw new Exception(String.Format("Organism already occupies coordinates ({0}, {1})", x, y));
            }

            this.niches[x][y].Organism = organism;
        }

        public void RemoveOrganism(int x, int y)
        {
            if (!this.ContainsOrganism(x, y))
            {
                throw new Exception(String.Format("No organism exists at coordinates ({0}, {1})", x, y));
            }

            this.niches[x][y].Organism = null;
        }

        public void MoveOrganismToRandomAvailableNiche(Organism organism)
        {
            var random = new Random();
            var targetColumn = random.Next(5);
            var targetRow = random.Next(5);

            if (!this.ContainsOrganism(targetColumn, targetRow))
            {
                var currentOrganismNiche = this.GetNicheOfOrganism(organism);
                currentOrganismNiche.Organism = null;

                this.AddOrganism(targetColumn, targetRow, organism);
            }
        }

        public List<Niche> GetOccupiedNiches()
        {
            var occupiedNiches = new List<Niche>();

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        occupiedNiches.Add(this.GetNiche(x, y));
                    }
                }
            }

            return occupiedNiches;
        }

        public List<Organism> GetOrganisms()
        {
            var organisms = new List<Organism>();

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        organisms.Add(this.Niches[x][y].Organism);
                    }
                }
            }

            return organisms;
        }

        private Niche GetNicheOfOrganism(Organism organism)
        {
            var nichesOfOrganism = new List<Niche>();

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.ContainsOrganism(x, y))
                    {
                        nichesOfOrganism.Add(this.Niches[x][y]);
                    }
                }
            }

            return nichesOfOrganism.Single();
        }

        public override String ToString()
        {
            return this.Width + " x " + this.Height;
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
