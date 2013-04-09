namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public sealed class Ecosystem
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Point> OrganismsAndLocations { get; set; } 

        public int Width
        {
            get
            {
                return this.Habitats.GetLength(0);
            }
        }

        public int Height
        {
            get
            {
                return this.Habitats.GetLength(1);
            }
        }

        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Point> organismsAndLocations)
        {
            this.Habitats = habitats;
            this.OrganismsAndLocations = organismsAndLocations;
        }

        public void Update()
        {
            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action

            var random = new Random();
            foreach (var organismsAndLocation in this.OrganismsAndLocations.ToList())
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
            this.Habitats[(int)source.X, (int)source.Y].Organism = null;
            this.Habitats[(int)destination.X, (int)destination.Y].Organism = organism;
            this.OrganismsAndLocations[organism] = destination;
        }

        public void AddOrganism(Organism organism, Point point)
        {
            this.Habitats[(int)point.X, (int)point.Y].Organism = organism;
            this.OrganismsAndLocations.Add(organism, point);
        }

        public void RemoveOrganism(Organism organism)
        {
            var location = this.OrganismsAndLocations[organism];
            this.Habitats[(int)location.X, (int)location.Y].Organism = null;
            this.OrganismsAndLocations.Remove(organism);
        }

        public void SetTerrain(int x, int y, Terrain terrain)
        {
            this.Habitats[x, y].Environment.Terrain = terrain;
        }

        // TODO: decide what this needs to return, that can be given to organisms for them to make decisions
        private IEnumerable<LocalArea> GetLocalAreaOfOccupiedHabitats()
        {
            var localAreaOfOccupiedHabitats = new List<LocalArea>();
            foreach (var organismsAndLocation in this.OrganismsAndLocations.ToList())
            {
                var xLocation = (int)organismsAndLocation.Value.X;
                var yLocation = (int)organismsAndLocation.Value.Y;

                var localAreaOfHabitat = new List<Habitat>();
                for (var x = xLocation - 1; x <= xLocation + 1; x++)
                {
                    if (x < 0 || x >= this.Width)
                    {
                        continue;
                    }
                    for (var y = yLocation - 1; y <= yLocation + 1; y++)
                    {
                        if (y < 0 || y >= this.Height)
                        {
                            continue;
                        }

                        localAreaOfHabitat.Add(this.Habitats[x, y]);
                    }
                }

                var localArea = new LocalArea(localAreaOfHabitat, this.Habitats[xLocation, yLocation]);
                localAreaOfOccupiedHabitats.Add(localArea);
            }

            return localAreaOfOccupiedHabitats;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1}", this.Width, this.Height);
        }
    }
}
