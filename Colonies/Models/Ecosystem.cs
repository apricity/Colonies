namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class Ecosystem
    {
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Coordinates> OrganismLocations { get; private set; }

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
        
        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismLocations)
        {
            this.Habitats = habitats;
            this.OrganismLocations = organismLocations;

            this.random = new Random();
        }

        public UpdateSummary Update()
        {
            /* record the pre-update locations */
            var preUpdate = this.OrganismLocations.ToDictionary(
                organismLocation => organismLocation.Key.ToString(), 
                organismLocation => organismLocation.Value);

            /* reduce pheromone level in all environments */
            this.DecreaseGlobalPheromoneLevel();

            /* reduce all organism's health */
            this.DecreaseAllOrganismHealth();

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to 
             * and to resolve any conflicting intentions */
            var intendedOrganismDestinations = this.GetIntendedOrganismDestinations();
            var actualOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations);

            /* perform in-situ actions e.g. take food, eat food, attack */

            /* perform ex-situ actions e.g. move any organisms that can after resolving conflicting intentions */
            foreach (var actualOrganismDestination in actualOrganismDestinations)
            {
                this.MoveOrganism(actualOrganismDestination.Key, actualOrganismDestination.Value);
            }

            /* record the post-update locations */
            var postUpdate = this.OrganismLocations.ToDictionary(
                organismLocation => organismLocation.Key.ToString(), 
                organismLocation => organismLocation.Value);

            return new UpdateSummary(preUpdate, postUpdate);
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
            foreach (var organismLocation in this.OrganismLocations.ToList())
            {
                var organism = organismLocation.Key;

                // reduce the organism's health / check if it is dead
                organism.DecreaseHealth(0.01);
                if (organism.Health.Equals(0))
                {
                    // it is dead!
                }
            }
        }

        private Dictionary<Organism, Coordinates> GetIntendedOrganismDestinations()
        {
            var intendedOrganismDestinations = new Dictionary<Organism, Coordinates>();
            foreach (var organismCoordinates in this.OrganismLocations.ToList())
            {
                var organism = organismCoordinates.Key;
                var location = organismCoordinates.Value;

                // get nearby stimuli
                var neighbourhoodStimuli = this.GetNeighbourhoodStimuli(location);

                // determine organism's intentions
                var chosenStimulus = organism.ProcessStimuli(neighbourhoodStimuli.Keys.ToList(), this.random);
                var intendedDestination = neighbourhoodStimuli[chosenStimulus];
                intendedOrganismDestinations.Add(organism, intendedDestination);
            }

            return intendedOrganismDestinations;
        }

        private Dictionary<Organism, Coordinates> ResolveOrganismDestinations(Dictionary<Organism, Coordinates> intendedOrganismDestinations)
        {
            var intendedDestinations = intendedOrganismDestinations.Values.ToList();
            var currentLocations = this.OrganismLocations.Values.ToList();

            // if there are no vacant destinations, this is our base case
            // return an empty list - none of the intended movements will become real movements
            var vacantDestinations = intendedDestinations.Except(currentLocations).ToList();
            if (vacantDestinations.Count == 0)
            {
                return new Dictionary<Organism, Coordinates>();
            }

            var resolvedOrganismDestinations = new Dictionary<Organism, Coordinates>();

            foreach (var destination in vacantDestinations)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantDestination = destination;
                var conflictingOrganisms = intendedOrganismDestinations
                    .Where(intendedOrganismDestination => intendedOrganismDestination.Value.Equals(vacantDestination))
                    .Select(intendedOrganismDestination => intendedOrganismDestination.Key)
                    .ToList();

                Organism organismToMove;
                if (conflictingOrganisms.Count > 1)
                {
                    // TODO: some logic to decide who will win the conflict
                    organismToMove = conflictingOrganisms.First();

                    // losers now intend to move nowhere
                    conflictingOrganisms.Remove(organismToMove);
                    foreach (var remainingOrganism in conflictingOrganisms)
                    {
                        intendedOrganismDestinations[remainingOrganism] = this.OrganismLocations[remainingOrganism];
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }
                
                // intended movement becomes an actual movement
                resolvedOrganismDestinations.Add(organismToMove, intendedOrganismDestinations[organismToMove]);
                intendedOrganismDestinations.Remove(organismToMove);
            }

            var trailingOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations);
            foreach (var trailingOrganismDestination in trailingOrganismDestinations)
            {
                resolvedOrganismDestinations.Add(trailingOrganismDestination.Key, trailingOrganismDestination.Value);
            }

            return resolvedOrganismDestinations;
        }

        private void MoveOrganism(Organism organism, Coordinates destination)
        {           
            var source = this.OrganismLocations[organism];

            if (organism.IsDepositingPheromones)
            {
                this.Habitats[source.X, source.Y].Environment.IncreasePheromoneLevel(0.01);
            }

            // the organism can only move to the destination if it does not already contain an organism
            if (!this.Habitats[destination.X, destination.Y].ContainsOrganism())
            {
                this.Habitats[source.X, source.Y].RemoveOrganism();
                this.Habitats[destination.X, destination.Y].AddOrganism(organism);
                this.OrganismLocations[organism] = destination;
            }
        }

        public void AddOrganism(Organism organism, Coordinates location)
        {
            this.Habitats[location.X, location.Y].AddOrganism(organism);
            this.OrganismLocations.Add(organism, location);
        }

        public void RemoveOrganism(Organism organism)
        {
            var location = this.OrganismLocations[organism];
            this.Habitats[location.X, location.Y].RemoveOrganism();
            this.OrganismLocations.Remove(organism);
        }

        public void SetTerrain(Coordinates location, Terrain terrain)
        {
            this.Habitats[location.X, location.Y].Environment.SetTerrain(terrain);
        }

        public void IncreasePheromoneLevel(Coordinates location, double levelIncrease)
        {
            this.Habitats[location.X, location.Y].Environment.IncreasePheromoneLevel(levelIncrease);
        }

        public void DecreasePheromoneLevel(Coordinates location, double levelDecrease)
        {
            this.Habitats[location.X, location.Y].Environment.DecreasePheromoneLevel(levelDecrease);
        }

        private Dictionary<Stimulus, Coordinates> GetNeighbourhoodStimuli(Coordinates location)
        {
            var neighbourhoodStimuli = new Dictionary<Stimulus, Coordinates>();
            for (var x = location.X - 1; x <= location.X + 1; x++)
            {
                // do not carry on if x is out-of-bounds
                if (x < 0 || x >= this.Width)
                {
                    continue;
                }

                for (var y = location.Y - 1; y <= location.Y + 1; y++)
                {
                    // do not carry on if y is out-of-bounds
                    if (y < 0 || y >= this.Height)
                    {
                        continue;
                    }

                    if (!this.Habitats[x, y].ContainsImpassable())
                    {
                        var currentLocation = new Coordinates(x, y);
                        neighbourhoodStimuli.Add(this.GetStimulus(currentLocation), currentLocation);
                    }
                }
            }

            return neighbourhoodStimuli;
        }

        private Stimulus GetStimulus(Coordinates location)
        {
            return this.Habitats[location.X, location.Y].GetStimulus();
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismLocations.Count);
        }
    }
}
