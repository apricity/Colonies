namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Colonies.Logic;

    public class Ecosystem : IBiased
    {
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Coordinates> OrganismLocations { get; private set; }
        public Dictionary<Habitat, Coordinates> HabitatLocations { get; private set; } 
        public double HealthBias { get; private set; }

        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismLocations)
        {
            this.Habitats = habitats;
            this.OrganismLocations = organismLocations;

            this.HealthBias = 1;

            this.HabitatLocations = new Dictionary<Habitat, Coordinates>();
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatLocations.Add(this.Habitats[i, j], new Coordinates(i, j));
                }
            }
        }

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
            var actualOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, new List<Organism>());

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
                habitat.Environment.DecreasePheromoneLevel(0.001);
            }
        }

        private void DecreaseAllOrganismHealth()
        {
            foreach (var organismLocation in this.OrganismLocations.ToList())
            {
                var organism = organismLocation.Key;
                organism.DecreaseHealth(0.01);
            }
        }

        protected virtual Dictionary<Organism, Coordinates> GetIntendedOrganismDestinations()
        {
            var intendedOrganismDestinations = new Dictionary<Organism, Coordinates>();
            foreach (var organismCoordinates in this.OrganismLocations.ToList())
            {
                var organism = organismCoordinates.Key;
                var location = organismCoordinates.Value;

                // get measurements of neighbouring environments
                var neighbouringHabitats = this.GetNeighbouringHabitats(location);

                // determine organism's intentions based on the measurements
                var chosenHabitat = DecisionLogic.MakeDecision(neighbouringHabitats, organism.GetMeasureBiases());
                var chosenCoordinate = this.HabitatLocations[chosenHabitat];
                
                intendedOrganismDestinations.Add(organism, chosenCoordinate);
            }

            return intendedOrganismDestinations;
        }

        private Dictionary<Organism, Coordinates> ResolveOrganismDestinations(Dictionary<Organism, Coordinates> intendedOrganismDestinations, IEnumerable<Organism> alreadyResolvedOrganisms)
        {
            var resolvedOrganismDestinations = new Dictionary<Organism, Coordinates>();

            // create a copy of the organism locations because we don't want to modify the actual set
            var currentOrganismLocations = this.OrganismLocations.ToDictionary(
                organismLocation => organismLocation.Key,
                organismLocation => organismLocation.Value);

            // remove organisms that have been resolved (from previous iterations)
            // as they no longer need to be processed
            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                currentOrganismLocations.Remove(alreadyResolvedOrganism);
            }

            var currentLocations = currentOrganismLocations.Values.ToList();
            var intendedDestinations = intendedOrganismDestinations.Values.ToList();

            // if there are no vacant destinations, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantDestinations = intendedDestinations.Except(currentLocations).ToList();
            if (vacantDestinations.Count == 0)
            {
                return resolvedOrganismDestinations;
            }

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
                    organismToMove = this.DecideOrganism(conflictingOrganisms);
                    conflictingOrganisms.Remove(organismToMove);

                    // the remaining conflicting organisms cannot move, so reset their intended destinations
                    foreach (var remainingOrganism in conflictingOrganisms)
                    {
                        intendedOrganismDestinations[remainingOrganism] = this.OrganismLocations[remainingOrganism];
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }
                
                // intended movement becomes an actual, resolved movement
                resolvedOrganismDestinations.Add(organismToMove, intendedOrganismDestinations[organismToMove]);
                intendedOrganismDestinations.Remove(organismToMove);
            }

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var resolvedOrganisms = resolvedOrganismDestinations.Keys.ToList();
            var trailingOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, resolvedOrganisms);
            foreach (var trailingOrganismDestination in trailingOrganismDestinations)
            {
                resolvedOrganismDestinations.Add(trailingOrganismDestination.Key, trailingOrganismDestination.Value);
            }

            return resolvedOrganismDestinations;
        }

        protected virtual Organism DecideOrganism(List<Organism> organisms)
        {
            // this is in a virtual method so the mock ecosystem can override for testing
            return DecisionLogic.MakeDecision(organisms, this.GetMeasureBiases());
        }

        private void MoveOrganism(Organism organism, Coordinates destination)
        {           
            var source = this.OrganismLocations[organism];
            if (organism.IsDepositingPheromones)
            {
                this.Habitats[source.X, source.Y].Environment.IncreasePheromoneLevel(0.01);
            }

            // the organism can only move to the destination if it does not already contain an organism
            if (this.Habitats[destination.X, destination.Y].ContainsOrganism())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is occupied by {2}",
                                  organism, destination, this.Habitats[destination.X, destination.Y].Organism));
            }

            this.Habitats[source.X, source.Y].RemoveOrganism();
            this.Habitats[destination.X, destination.Y].AddOrganism(organism);
            this.OrganismLocations[organism] = destination;
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


        private List<Habitat> GetNeighbouringHabitats(Coordinates location)
        {
            var neighbouringHabitats = new List<Habitat>();
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

                    var currentHabitat = this.Habitats[x, y];
                    if (!currentHabitat.ContainsImpassable())
                    {
                        neighbouringHabitats.Add(currentHabitat);
                    }
                }
            }

            return neighbouringHabitats;
        }

        public Dictionary<Measure, double> GetMeasureBiases()
        {
            return new Dictionary<Measure, double> { { Measure.Health, this.HealthBias } };
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismLocations.Count);
        }
    }
}