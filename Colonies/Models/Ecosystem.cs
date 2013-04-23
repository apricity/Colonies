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

        public UpdateSummary Update()
        {
            var preUpdate = new Dictionary<string, Coordinates>();
            var postUpdate = new Dictionary<string, Coordinates>();

            foreach (var organismCoordinate in OrganismCoordinates)
            {
                // record the pre-update location
                preUpdate.Add(organismCoordinate.Key.ToString(), organismCoordinate.Value);
            }

            /* reduce pheromone level in all environments */
            this.DecreaseGlobalPheromoneLevel();

            /* reduce all organism's health */
            this.DecreaseAllOrganismHealth();

            /* get organisms intentions */
            var intendedMovements = this.GetIntendedOrganismMovements();

            /* resolve clashing intentions */
            var actualMovements = this.GetActualMovements(intendedMovements);

            /* perform in-situ actions e.g. take food, eat food, attack */

            /* perform ex-situ actions e.g. move any organisms that can after resolving clashing intentions */
            foreach (var actualMovement in actualMovements)
            {
                this.MoveOrganism(actualMovement.Key, actualMovement.Value);

                // record the pre-update location
                postUpdate.Add(actualMovement.Key.ToString(), actualMovement.Value);
            }

            return new UpdateSummary(preUpdate, postUpdate);
        }

        private Dictionary<Organism, Coordinates> GetActualMovements(Dictionary<Organism, Coordinates> intendedMovements)
        {
            var availableDestinations = intendedMovements.Values.Except(this.OrganismCoordinates.Values).ToList();
            if (availableDestinations.Count == 0)
            {
                return null;
            }

            var actualMovements = new Dictionary<Organism, Coordinates>();

            foreach (var availableDestination in availableDestinations)
            {
                var destination = availableDestination;
                var organismsWantingDestination = intendedMovements.Where(item => item.Value.Equals(destination)).ToList();

                KeyValuePair<Organism, Coordinates> organismToMove;
                if (intendedMovements.Values.Count(value => value.Equals(availableDestination)) > 1)
                {
                    // work out who will win
                    organismToMove = organismsWantingDestination.First();

                    // losers now intend to move nowhere
                    organismsWantingDestination.Remove(organismToMove);
                    foreach (var remainingOrganism in organismsWantingDestination)
                    {
                        intendedMovements[remainingOrganism.Key] = this.OrganismCoordinates[remainingOrganism.Key];
                    }
                }
                else
                {
                    organismToMove = organismsWantingDestination.Single();
                }
                
                // intended movement becomes an actual movement
                actualMovements.Add(organismToMove.Key, intendedMovements[organismToMove.Key]);
                intendedMovements.Remove(organismToMove.Key);
            }

            var childMovements = this.GetActualMovements(intendedMovements);
            if (childMovements != null)
            {
                foreach (var childMovement in childMovements)
                {
                    actualMovements.Add(childMovement.Key, childMovement.Value);
                }
            }

            return actualMovements;
        }

        private Dictionary<Organism, Coordinates> GetIntendedOrganismMovements()
        {
            var intendedMovements = new Dictionary<Organism, Coordinates>();
            foreach (var organismCoordinates in this.OrganismCoordinates.ToList())
            {
                var organism = organismCoordinates.Key;
                var location = organismCoordinates.Value;

                // get nearby stimuli
                var neighbourhoodStimuli = this.GetNeighbourhoodStimuli(location);

                // determine organism's intentions
                var chosenStimulus = organism.ProcessStimuli(neighbourhoodStimuli.Keys.ToList(), this.random);
                var destination = neighbourhoodStimuli[chosenStimulus];
                intendedMovements.Add(organism, destination);
            }

            return intendedMovements;
        }

        private void DecreaseGlobalPheromoneLevel()
        {
            foreach (var habitat in this.Habitats)
            {
                if (habitat.Environment.PheromoneLevel > 0)
                {
                    habitat.Environment.DecreasePheromoneLevel(0.001);
                }
            }
        }

        private void DecreaseAllOrganismHealth()
        {
            foreach (var organismCoordinates in this.OrganismCoordinates.ToList())
            {
                var organism = organismCoordinates.Key;

                // reduce the organism's health / check if it is dead
                organism.DecreaseHealth(0.01);
                if (organism.Health.Equals(0))
                {
                    // it is dead!
                }
            }
        }

        private void MoveOrganism(Organism organism, Coordinates destination)
        {           
            var source = this.OrganismCoordinates[organism];

            if (organism.IsDepositingPheromones)
            {
                this.Habitats[source.X, source.Y].Environment.IncreasePheromoneLevel(0.01);
            }

            // the organism can only move to the destination if it does not already contain an organism
            if (!this.Habitats[destination.X, destination.Y].ContainsOrganism())
            {
                this.Habitats[source.X, source.Y].RemoveOrganism();
                this.Habitats[destination.X, destination.Y].AddOrganism(organism);
                this.OrganismCoordinates[organism] = destination;
            }
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

                    if (!this.Habitats[x, y].ContainsImpassable())
                    {
                        var currentCoordinates = new Coordinates(x, y);
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
