namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Ecosystem
    {
        private Habitat[,] Habitats { get; set; }
        private Dictionary<Organism, Coordinates> OrganismCoordinates { get; set; } 

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

        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismCoordinates)
        {
            this.Habitats = habitats;
            this.OrganismCoordinates = organismCoordinates;
        }

        public void Update()
        {
            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action

            var random = new Random();
            foreach (var organismsAndLocation in this.OrganismCoordinates.ToList())
            {
                /* decide what to do */
                var decision = organismsAndLocation.Key.TakeTurn(null);

                var randomX = random.Next(this.Width);
                var randomY = random.Next(this.Height);
                var destination = new Coordinates(randomX, randomY);

                this.MoveOrganism(organismsAndLocation.Key, destination);
            }
        }

        private void MoveOrganism(Organism organism, Coordinates destination)
        {
            // use Add and Remove methods?
            var source = this.OrganismCoordinates[organism];
            this.Habitats[source.X, source.Y].Organism = null;
            this.Habitats[destination.X, destination.Y].Organism = organism;
            this.OrganismCoordinates[organism] = destination;
        }

        public void AddOrganism(Organism organism, Coordinates coordinates)
        {
            this.Habitats[coordinates.X, coordinates.Y].Organism = organism;
            this.OrganismCoordinates.Add(organism, coordinates);
        }

        public void RemoveOrganism(Organism organism)
        {
            var location = this.OrganismCoordinates[organism];
            this.Habitats[location.X, location.Y].Organism = null;
            this.OrganismCoordinates.Remove(organism);
        }

        public void SetTerrain(int x, int y, Terrain terrain)
        {
            this.Habitats[x, y].Environment.Terrain = terrain;
        }

        // TODO: decide what this needs to return, that can be given to organisms for them to make decisions
        private IEnumerable<LocalArea> GetLocalAreaOfOccupiedHabitats()
        {
            var localAreaOfOccupiedHabitats = new List<LocalArea>();
            foreach (var organismsAndLocation in this.OrganismCoordinates.ToList())
            {
                var xLocation = organismsAndLocation.Value.X;
                var yLocation = organismsAndLocation.Value.Y;

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
