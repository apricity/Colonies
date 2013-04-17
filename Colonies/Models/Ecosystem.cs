namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Ecosystem
    {
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Coordinates> OrganismCoordinates { get; private set; }

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

        private readonly Random random;
        
        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismCoordinates)
        {
            this.Habitats = habitats;
            this.OrganismCoordinates = organismCoordinates;

            this.random = new Random();
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

                /* reduce pheromone level in all environments */
                foreach (var habitat in this.Habitats)
                {
                    if (habitat.Environment.PheromoneLevel > 0)
                    {
                        habitat.Environment.DecreasePheromoneLevel(0.0001);
                    }
                }

                /* record the pre-update location */
                preUpdate.Add(organism.ToString(), location);

                /* get nearby stimuli */
                var neighbourhoodStimuli = this.GetNeighbourhoodStimuli(location);

                /* decide what to do */
                var chosenStimulus = organism.ProcessStimuli(neighbourhoodStimuli.Keys.ToList(), this.random);
                var destination = neighbourhoodStimuli[chosenStimulus];

                /* take action based on decision */
                this.MoveOrganism(organism, destination);

                /* record the post-update location */
                postUpdate.Add(organism.ToString(), destination);
            }

            return new UpdateSummary(preUpdate, postUpdate);
        }

        private void MoveOrganism(Organism organism, Coordinates destination)
        {           
            var source = this.OrganismCoordinates[organism];
            if (organism.IsDepositingPheromones)
            {
                this.Habitats[source.X, source.Y].Environment.IncreasePheromoneLevel(0.01);
            }

            this.Habitats[source.X, source.Y].RemoveOrganism();
            this.Habitats[destination.X, destination.Y].AddOrganism(organism);
            this.OrganismCoordinates[organism] = destination;
        }

        public void AddOrganism(Organism organism, Coordinates coordinates)
        {
            this.Habitats[coordinates.X, coordinates.Y].AddOrganism(organism);
            this.OrganismCoordinates.Add(organism, coordinates);
        }

        public void RemoveOrganism(Organism organism)
        {
            var location = this.OrganismCoordinates[organism];
            this.Habitats[location.X, location.Y].RemoveOrganism();
            this.OrganismCoordinates.Remove(organism);
        }

        public void SetTerrain(Coordinates coordinates, Terrain terrain)
        {
            this.Habitats[coordinates.X, coordinates.Y].Environment.SetTerrain(terrain);
        }

        public void IncreasePheromoneLevel(Coordinates coordinates, double levelIncrease)
        {
            this.Habitats[coordinates.X, coordinates.Y].Environment.IncreasePheromoneLevel(levelIncrease);
        }

        public void DecreasePheromoneLevel(Coordinates coordinates, double levelDecrease)
        {
            this.Habitats[coordinates.X, coordinates.Y].Environment.DecreasePheromoneLevel(levelDecrease);
        }

        private Dictionary<Stimulus, Coordinates> GetNeighbourhoodStimuli(Coordinates coordinates)
        {
            var neighbourhoodStimuli = new Dictionary<Stimulus, Coordinates>();
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
                    if (Habitats[x, y].ContainsImpassable())
                    {
                        break;
                    }
                    else
                    {
                        neighbourhoodStimuli.Add(this.GetStimulus(currentCoordinates), currentCoordinates);
                    }
                }
            }

            return neighbourhoodStimuli;
        }

        private Stimulus GetStimulus(Coordinates coordinates)
        {
            return this.Habitats[coordinates.X, coordinates.Y].GetStimulus();
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismCoordinates.Count);
        }
    }
}
