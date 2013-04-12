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
        public UpdateSummary Update()
        {
            var preUpdate = new Dictionary<string, Coordinates>();
            var postUpdate = new Dictionary<string, Coordinates>();

            // TODO: all organisms should return an INTENTION of what they would like to do
            // TODO: then we should check for clashes before proceeding with the movement/action
            foreach (var organismCoordinates in this.OrganismCoordinates.ToList())
            {
                var organism = organismCoordinates.Key;
                var location = organismCoordinates.Value;

                /* record the pre-update location */
                preUpdate.Add(organism.ToString(), location);

                /* get nearby habitat conditions */
                var adjacentHabitatConditions = this.GetAdjacentHabitatConditions(location);

                /* decide what to do */
                var chosenHabitatCondition = organism.TakeTurn(adjacentHabitatConditions.Keys.ToList(), this.randomNumberGenerator);
                var destination = adjacentHabitatConditions[chosenHabitatCondition];

                /* take action based on decision */
                this.MoveOrganism(organism, destination);

                /* record the post-update location */
                postUpdate.Add(organism.ToString(), destination);
            }

            return new UpdateSummary(preUpdate, postUpdate);
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

        public void IncreasePheromoneLevel(Coordinates coordinates, double levelIncrease)
        {
            this.Habitats[coordinates.X, coordinates.Y].Environment.IncreasePheromoneLevel(levelIncrease);
        }

        public void DecreasePheromoneLevel(Coordinates coordinates, double levelDecrease)
        {
            this.Habitats[coordinates.X, coordinates.Y].Environment.IncreasePheromoneLevel(levelDecrease);
        }

        private Dictionary<HabitatCondition, Coordinates> GetAdjacentHabitatConditions(Coordinates coordinates)
        {
            var habitatConditions = new Dictionary<HabitatCondition, Coordinates>();
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
                    habitatConditions.Add(this.GetHabitatCondition(currentCoordinates), currentCoordinates);
                }
            }

            return habitatConditions;
        }

        private HabitatCondition GetHabitatCondition(Coordinates coordinates)
        {
            return this.Habitats[coordinates.X, coordinates.Y].GetCondition();
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismCoordinates.Count);
        }
    }
}
