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

        private readonly Random randomNumberGenerator;

        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismCoordinates)
        {
            this.Habitats = habitats;
            this.OrganismCoordinates = organismCoordinates;

            this.randomNumberGenerator = new Random();
        }

        // TODO: return a summary of what has happened so we know which habitat view models to update
        public void Update()
        {
            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action
            foreach (var organismCoordinates in this.OrganismCoordinates.ToList())
            {
                /* get nearby habitat features */
                var nearbyHabitatFeatures = this.GetNearbyHabitatFeatures(organismCoordinates.Value);

                /* decide what to do */
                var chosenFeature = organismCoordinates.Key.TakeTurn(nearbyHabitatFeatures.Keys.ToList(), this.randomNumberGenerator);
                var destinationCoordinates = nearbyHabitatFeatures[chosenFeature];

                /* take action based on decision */
                this.MoveOrganism(organismCoordinates.Key, destinationCoordinates);
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

        public void SetTerrain(Coordinates coordinates, Terrain terrain)
        {
            this.Habitats[coordinates.X, coordinates.Y].Environment.Terrain = terrain;
        }

        private Dictionary<Features, Coordinates> GetNearbyHabitatFeatures(Coordinates coordinates)
        {
            var nearbyHabitatFeatures = new Dictionary<Features, Coordinates>();
            for (var x = coordinates.X - 1; x <= coordinates.X + 1; x++)
            {
                // do not carry on if x is out-of-bounds
                if (x < 0 || x >= this.Width)
                {
                    continue;
                }

                for (var y = coordinates.Y - 1; y <= coordinates.Y + 1; y++)
                {
                    // do not carry on if y is out-of-bounds
                    if (y < 0 || y >= this.Height)
                    {
                        continue;
                    }

                    var currentCoordinates = new Coordinates(x, y);
                    nearbyHabitatFeatures.Add(this.GetHabitatFeatures(currentCoordinates), currentCoordinates);
                }
            }

            return nearbyHabitatFeatures;
        }

        private Features GetHabitatFeatures(Coordinates coordinates)
        {
            return this.Habitats[coordinates.X, coordinates.Y].GetFeatures();
        }

        public override String ToString()
        {
            return string.Format("{0}x{1}", this.Width, this.Height);
        }
    }
}
