namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Ecosystem
    {
        public List<List<Habitat>> Habitats { get; set; }

        public int Height
        {
            get
            {
                return this.Habitats.First().Count;
            }
        }

        public int Width
        {
            get
            {
                return this.Habitats.Count;
            }
        }

        public Ecosystem(List<List<Habitat>> habitats)
        {
            this.Habitats = habitats;
        }

        public void Update()
        {
            // TODO: WAFFLE
            // currently, updating the ecosystem simply means
            // randomly moving all organisms to a different habitat
            var random = new Random();

            var localAreas = this.GetLocalAreaOfOccupiedHabitats();
            foreach (var localArea in localAreas)
            {
                var destination = random.Next(localArea.Size());
                var destinationHabitat = localArea.LocalHabitats[destination];

                if (!destinationHabitat.Equals(localArea.HabitatOfFocus) && !destinationHabitat.ContainsOrganism())
                {
                    this.MoveOrganism(localArea.HabitatOfFocus, destinationHabitat);
                }
            }
        }

        private void MoveOrganism(Habitat sourceHabitat, Habitat destinationHabitat)
        {
            if (!sourceHabitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("Source habitat {0} does not contain an organism", sourceHabitat), "sourceHabitat");
            }

            if (destinationHabitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("Destination habitat {0} already contains an organism", destinationHabitat), "destinationHabitat");
            }

            var organismToMove = sourceHabitat.Organism;
            this.RemoveOrganism(sourceHabitat);
            this.AddOrganism(destinationHabitat, organismToMove);
        }

        private bool ContainsOrganism(int x, int y)
        {
            return this.Habitats[x][y].ContainsOrganism();
        }

        private void AddOrganism(Habitat habitat, Organism organism)
        {
            if (habitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("Organism already exists at habitat {0}", habitat), "habitat");
            }

            habitat.Organism = organism;
        }

        private void RemoveOrganism(Habitat habitat)
        {
            if (!habitat.ContainsOrganism())
            {
                throw new ArgumentException(String.Format("No organism exists at habitat {0}", habitat), "habitat");
            }

            habitat.Organism = null;
        }

        private IEnumerable<Habitat> GetOccupiedHabitats()
        {
            var occupiedHabitats = new List<Habitat>();
            foreach (var habitat in Habitats.SelectMany(item => item))
            {
                if (habitat.ContainsOrganism())
                {
                    occupiedHabitats.Add(habitat);
                }
            }

            return occupiedHabitats;
        }

        private IEnumerable<LocalArea> GetLocalAreaOfOccupiedHabitats()
        {
            var localAreaOfOccupiedHabitats = new List<LocalArea>();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.Habitats[x][y].ContainsOrganism())
                    {
                        var localAreaOfHabitat = new List<Habitat>();
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            if (i < 0 || i >= this.Width)
                            {
                                continue;
                            }
                            for (int j = y - 1; j <= y + 1; j++)
                            {
                                if (j < 0 || j >= this.Height)
                                {
                                    continue;
                                }

                                localAreaOfHabitat.Add(Habitats[i][j]);
                            }
                        }
                        var localArea = new LocalArea(localAreaOfHabitat,this.Habitats[x][y]);
                        localAreaOfOccupiedHabitats.Add(localArea);
                    }
                }
            }

            return localAreaOfOccupiedHabitats;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1}", this.Width, this.Height);
        }
    }
}
