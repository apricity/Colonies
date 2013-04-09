namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public sealed class Ecosystem
    {
        public List<List<Habitat>> Habitats { get; set; }
        private Dictionary<Organism, Point> OrganismsAndLocations { get; set; } 

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

        public Ecosystem(List<List<Habitat>> habitats, Dictionary<Organism, Point> organismsAndLocations)
        {
            this.Habitats = habitats;
            this.OrganismsAndLocations = organismsAndLocations;
        }

        public void Update()
        {
            // organisms are now the things that make the decisions about where to move
            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action

            var random = new Random();
            foreach (var organismsAndLocation in OrganismsAndLocations.ToList())
            {
                /* decide what to do */
                var decision = organismsAndLocation.Key.TakeTurn(null);

                var randomX = random.Next(this.Width);
                var randomY = random.Next(this.Height);
                var destination = new Point(randomX, randomY);

                this.MoveOrganism(organismsAndLocation.Key, destination);
            }
        }

        private void MoveOrganism(Organism organism, Point destination)
        {
            // use Add and Remove methods?
            var source = this.OrganismsAndLocations[organism];
            this.Habitats[(int)source.X][(int)source.Y].Organism = null;
            this.Habitats[(int)destination.X][(int)destination.Y].Organism = organism;
            this.OrganismsAndLocations[organism] = destination;
        }

        public void AddOrganism(Organism organism, Point point)
        {
            this.Habitats[(int)point.X][(int)point.Y].Organism = organism;
            this.OrganismsAndLocations.Add(organism, point);
        }

        public void RemoveOrganism(Organism organism)
        {
            var location = this.OrganismsAndLocations[organism];
            this.Habitats[(int)location.X][(int)location.Y].Organism = null;
            this.OrganismsAndLocations.Remove(organism);
        }

        // TODO: modify environments


        // TODO: holy hells, this needs some thought...
        private Dictionary<Habitat, List<Habitat>> GetLocalAreasOfOrganisms()
        {
            var localAreaOfOrganisms = new Dictionary<Habitat, List<Habitat>>();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    if (this.Habitats[x][y].ContainsOrganism())
                    {
                        var currentHabitat = this.Habitats[x][y];

                        // TODO [Waff]: refactor this into its own method
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
                        var localArea = new LocalArea(localAreaOfHabitat, this.Habitats[x][y]);
                        localAreaOfOrganisms.Add(currentHabitat, localArea.LocalHabitats);
                    }
                }
            }

            return localAreaOfOrganisms;
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
                        // TODO [Waff]: refactor this into its own method
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
