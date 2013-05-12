namespace Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Colonies.Logic;

    public sealed class Ecosystem : IBiased
    {
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Coordinates> OrganismLocations { get; private set; }

        private readonly IDecisionLogic conflictingMovementLogic;

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

        public Dictionary<Measure, double> MeasureBiases { get; private set; }
        
        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Coordinates> organismLocations, IDecisionLogic conflictingMovementLogic)
        {
            this.Habitats = habitats;
            this.OrganismLocations = organismLocations;
            this.conflictingMovementLogic = conflictingMovementLogic;

            this.random = new Random();

            this.MeasureBiases = new Dictionary<Measure, double> { { Measure.Health, 1 } };
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
            // TODO: should there be another layer of logic - organism movement logic (which contains conflict movement logic)?
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
                if (habitat.Environment.Pheromone.Level > 0)
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
                if (organism.Health.Level.Equals(0))
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

                // get measurements of neighbouring environments
                var neighbourhoodEnvironmentMeasurements = this.GetNeighbourhoodEnvironmentMeasurements(location);

                // determine organism's intentions based on the measurements
                var chosenMeasurement = organism.ProcessEnvironmentMeasurements(neighbourhoodEnvironmentMeasurements.Keys.ToList(), this.random);
                var intendedDestination = neighbourhoodEnvironmentMeasurements[chosenMeasurement];
                intendedOrganismDestinations.Add(organism, intendedDestination);
            }

            return intendedOrganismDestinations;
        }

        private Dictionary<Organism, Coordinates> ResolveOrganismDestinations(Dictionary<Organism, Coordinates> intendedOrganismDestinations, IEnumerable<Organism> alreadyResolvedOrganisms)
        {
            var currentOrganismLocations = this.OrganismLocations.ToDictionary(
                organismLocation => organismLocation.Key,
                organismLocation => organismLocation.Value);

            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                currentOrganismLocations.Remove(alreadyResolvedOrganism);
            }

            var currentLocations = currentOrganismLocations.Values.ToList();
            var intendedDestinations = intendedOrganismDestinations.Values.ToList();
            var resolvedOrganismDestinations = new Dictionary<Organism, Coordinates>();

            // if there are no vacant destinations, this is our base case
            // return an empty list - none of the intended movements will become real movements
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
                    // TODO: tidy up/make generic this KVP of <Measurement, thing to choose>
                    var conflictingOrganismMeasurements = new Dictionary<Measurement, Organism>();
                    foreach (var conflictingOrganism in conflictingOrganisms)
                    {
                        conflictingOrganismMeasurements.Add(conflictingOrganism.GetMeasurement(), conflictingOrganism);
                    }

                    var chosenStimulus = this.ProcessOrganismMeasurements(conflictingOrganismMeasurements, this.random);
                    organismToMove = conflictingOrganismMeasurements[chosenStimulus];

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

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var trailingOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, resolvedOrganismDestinations.Keys.ToList());
            foreach (var trailingOrganismDestination in trailingOrganismDestinations)
            {
                resolvedOrganismDestinations.Add(trailingOrganismDestination.Key, trailingOrganismDestination.Value);
            }

            return resolvedOrganismDestinations;
        }

        // TODO: extract "ProcessXxxMeasurements" (here and in Organism - IBiased classes)
        private Measurement ProcessOrganismMeasurements(Dictionary<Measurement, Organism> conflictingOrganismMeasurements, Random random)
        {
            // add bias to the measurements, according to how the organism weights each measure
            foreach (var conflictingOrganismMeasurement in conflictingOrganismMeasurements.Keys.ToList())
            {
                foreach (var condition in conflictingOrganismMeasurement.Conditions)
                {
                    condition.SetBias(this.MeasureBiases[condition.Measure]);
                }
            }

            var chosenMeasurement = this.conflictingMovementLogic.MakeDecision(conflictingOrganismMeasurements.Keys.ToList(), random);
            return chosenMeasurement;
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

        private Dictionary<Measurement, Coordinates> GetNeighbourhoodEnvironmentMeasurements(Coordinates location)
        {
            var neighbourhoodMeasurements = new Dictionary<Measurement, Coordinates>();
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
                        neighbourhoodMeasurements.Add(this.GetEnvironmentMeasurement(currentLocation), currentLocation);
                    }
                }
            }

            return neighbourhoodMeasurements;
        }

        private Measurement GetEnvironmentMeasurement(Coordinates location)
        {
            return this.Habitats[location.X, location.Y].GetEnvironmentMeasurement();
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismLocations.Count);
        }
    }
}