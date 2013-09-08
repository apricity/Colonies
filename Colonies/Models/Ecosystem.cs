namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Logic;

    public class Ecosystem : IBiased
    {
        public Habitat[,] Habitats { get; private set; }
        public Dictionary<Organism, Habitat> OrganismHabitats { get; private set; }
        public Dictionary<Habitat, Coordinates> HabitatCoordinates { get; private set; } 
        public Dictionary<Measure, double> MeasureBiases { get; private set; }

        // TODO: neater management of these?
        public double HealthDeteriorationRate { get; set; }
        public double PheromoneDepositRate { get; set; }
        public double PheromoneFadeRate { get; set; }
        public double NutrientGrowthRate { get; set; }
        public double MineralGrowthRate { get; set; }
        public double ObstructionDemolishRate { get; set; }

        public Ecosystem(Habitat[,] habitats, Dictionary<Organism, Habitat> organismHabitats)
        {
            this.Habitats = habitats;
            this.OrganismHabitats = organismHabitats;

            this.HabitatCoordinates = new Dictionary<Habitat, Coordinates>();
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinates(i, j));
                }
            }

            this.MeasureBiases = new Dictionary<Measure, double> { { Measure.Health, 1 } };

            this.HealthDeteriorationRate = 1 / 500.0;
            this.PheromoneDepositRate = 1 / 100.0;
            this.PheromoneFadeRate = 1 / 500.0;
            this.NutrientGrowthRate = 1 / 500.0;
            this.MineralGrowthRate = 1 / 750.0;
            this.ObstructionDemolishRate = 1 / 5.0;
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
            var preUpdateOrganismLocations = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key.ToString(), 
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);

            /* reduce pheromone level in all environments and increase pheromone wherever appropriate */
            var pheromoneDecreasedLocations = this.DecreaseGlobalPheromoneLevel();
            this.IncreaseLocalPheromoneLevels();

            /* increase nutrient and mineral growth wherever appropriate */
            var nutrientGrowthLocations = this.IncreaseNutrientLevels();
            var mineralGrowthLocations = this.IncreaseMineralLevels();

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to 
             * and to resolve any conflicting intentions */
            var intendedOrganismDestinations = this.GetIntendedOrganismDestinations();
            var actualOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, new List<Organism>());

            foreach (var obstructedHabitat in intendedOrganismDestinations.Values.Where(habitat => habitat.IsObstructed()))
            {
                obstructedHabitat.Environment.DecreaseLevel(Measure.Obstruction, this.ObstructionDemolishRate);
            }

            /* perform in-situ actions e.g. take food, eat food, attack */
            foreach (var actualOrganismDestination in actualOrganismDestinations)
            {
                var organism = actualOrganismDestination.Key;
                var habitat = actualOrganismDestination.Value;
                var habitatNutrientLevel = habitat.Environment.GetLevel(Measure.Nutrient);

                if (habitatNutrientLevel > 0.0)
                {
                    var desiredNutrients = 1 - organism.GetLevel(Measure.Health);
                    var nutrientTaken = Math.Min(desiredNutrients, habitatNutrientLevel);

                    organism.IncreaseLevel(Measure.Health, nutrientTaken);
                    habitat.Environment.DecreaseLevel(Measure.Nutrient, nutrientTaken);
                }
            }

            /* perform ex-situ actions e.g. move any organisms that can after resolving conflicting intentions */
            foreach (var actualOrganismDestination in actualOrganismDestinations)
            {
                this.MoveOrganism(actualOrganismDestination.Key, actualOrganismDestination.Value);                    
            }

            /* reduce all organisms health */
            this.DecreaseAllOrganismHealth();

            /* record the post-update locations */
            var postUpdateOrganismLocations = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key.ToString(), 
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);

            return new UpdateSummary(preUpdateOrganismLocations, postUpdateOrganismLocations, pheromoneDecreasedLocations, nutrientGrowthLocations, mineralGrowthLocations);
        }

        private List<Coordinates> DecreaseGlobalPheromoneLevel()
        {
            var pheromoneDecreasedLocations = new List<Coordinates>();

            foreach (var habitat in this.Habitats)
            {
                if (habitat.Environment.DecreaseLevel(Measure.Pheromone,  this.PheromoneFadeRate))
                {
                    pheromoneDecreasedLocations.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return pheromoneDecreasedLocations;
        }

        private void IncreaseLocalPheromoneLevels()
        {
            foreach (var organismHabitat in this.OrganismHabitats)
            {
                var organism = organismHabitat.Key;
                var habitat = organismHabitat.Value;

                if (organism.IsDepositingPheromones && organism.IsAlive)
                {
                    habitat.Environment.IncreaseLevel(Measure.Pheromone, this.PheromoneDepositRate);
                }
            }
        }

        private List<Coordinates> IncreaseNutrientLevels()
        {
            var nutrientGrowthLocations = new List<Coordinates>();

            foreach (var habitat in this.Habitats)
            {
                if (!habitat.Environment.HasNutrient)
                {
                    continue;
                }

                var increased = habitat.Environment.IncreaseLevel(Measure.Nutrient, this.NutrientGrowthRate);
                if (increased)
                {
                    nutrientGrowthLocations.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return nutrientGrowthLocations;
        }

        private List<Coordinates> IncreaseMineralLevels()
        {
            var mineralGrowthLocations = new List<Coordinates>();

            foreach (var habitat in this.Habitats)
            {
                if (!habitat.Environment.Terrain.Equals(Terrain.Earth))
                {
                    continue;
                }

                var increased = habitat.Environment.IncreaseLevel(Measure.Mineral, this.MineralGrowthRate);
                if (increased)
                {
                    mineralGrowthLocations.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return mineralGrowthLocations;
        }

        private void DecreaseAllOrganismHealth()
        {
            foreach (var organism in this.OrganismHabitats.Keys.ToList())
            {
                organism.DecreaseLevel(Measure.Health, this.HealthDeteriorationRate);
            }
        }

        public void InsertWater(Coordinates coordinates)
        {
            var waterHabitat = this.Habitats[coordinates.X, coordinates.Y];
            waterHabitat.Environment.SetLevel(Measure.Damp, 1.0);
            waterHabitat.Environment.SetTerrain(Terrain.Water);

            var neighbouringHabitats = this.GetNeighbouringHabitats(waterHabitat);
            foreach (var habitat in neighbouringHabitats)
            {
                if (habitat.Equals(waterHabitat))
                {
                    continue;
                }

                habitat.Environment.SetLevel(Measure.Damp, 0.5);
            }
        }

        public void InsertFire(Coordinates coordinates)
        {
            var fireHabitat = this.Habitats[coordinates.X, coordinates.Y];
            fireHabitat.Environment.SetLevel(Measure.Heat, 1.0);
            fireHabitat.Environment.SetTerrain(Terrain.Fire);

            var neighbouringHabitats = this.GetNeighbouringHabitats(fireHabitat);
            foreach (var habitat in neighbouringHabitats)
            {
                if (habitat.Equals(fireHabitat))
                {
                    continue;
                }

                habitat.Environment.SetLevel(Measure.Heat, 0.5);
            }
        }

        protected virtual Dictionary<Organism, Habitat> GetIntendedOrganismDestinations()
        {
            var intendedOrganismDestinations = new Dictionary<Organism, Habitat>();
            var aliveOrganismHabitats = this.OrganismHabitats.Where(organismHabitats => organismHabitats.Key.IsAlive).ToList();
            foreach (var organismCoordinates in aliveOrganismHabitats)
            {
                var organism = organismCoordinates.Key;
                var habitat = organismCoordinates.Value;

                // get measurements of neighbouring environments
                var neighbouringHabitats = this.GetNeighbouringHabitats(habitat);
                var neighbouringEnvironments = neighbouringHabitats.Select(neighbour => neighbour.Environment).ToList();

                // determine organism's intentions based on the environment measurements
                var chosenEnvironment = DecisionLogic.MakeDecision(neighbouringEnvironments, organism);

                // get the habitat the environment is from - this is where the organism wants to move to
                var chosenHabitat = neighbouringHabitats.Single(neighbour => neighbour.Environment.Equals(chosenEnvironment));
                intendedOrganismDestinations.Add(organism, chosenHabitat);
            }

            return intendedOrganismDestinations;
        }

        private Dictionary<Organism, Habitat> ResolveOrganismDestinations(Dictionary<Organism, Habitat> intendedOrganismDestinations, IEnumerable<Organism> alreadyResolvedOrganisms)
        {
            var resolvedOrganismDestinations = new Dictionary<Organism, Habitat>();

            // create a copy of the organism habitats because we don't want to modify the actual set
            var currentOrganismHabitats = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key,
                organismHabitat => organismHabitat.Value);

            // remove organisms that have been resolved (from previous iterations)
            // as they no longer need to be processed
            foreach (var alreadyResolvedOrganism in alreadyResolvedOrganisms)
            {
                currentOrganismHabitats.Remove(alreadyResolvedOrganism);
            }

            var occupiedHabitats = currentOrganismHabitats.Values.ToList();
            var intendedHabitats = intendedOrganismDestinations.Values.ToList();

            // if there are no vacant habitats, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantHabitats = intendedHabitats.Except(occupiedHabitats).Where(habitat => !habitat.IsObstructed()).ToList();
            if (vacantHabitats.Count == 0)
            {
                return resolvedOrganismDestinations;
            }

            foreach (var habitat in vacantHabitats)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantHabitat = habitat;
                var conflictingOrganisms = intendedOrganismDestinations
                    .Where(intendedOrganismDestination => intendedOrganismDestination.Value.Equals(vacantHabitat))
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
                        intendedOrganismDestinations[remainingOrganism] = this.OrganismHabitats[remainingOrganism];
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
            return DecisionLogic.MakeDecision(organisms, this);
        }

        private void MoveOrganism(Organism organism, Habitat destination)
        {           
            var source = this.OrganismHabitats[organism];

            // the organism cannot move if it is dead
            if (!organism.IsAlive)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because it is dead",
                                   organism, destination));
            }

            // the organism can only move to the destination if it is not obstructed
            if (destination.IsObstructed())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is obstructed", 
                                  organism, destination));
            }

            // the organism can only move to the destination if it does not already contain an organism
            if (destination.ContainsOrganism())
            {
                throw new InvalidOperationException(
                    string.Format("Cannot move organism {0} to {1} because the destination is occupied by {2}",
                                  organism, destination, destination.Organism));
            }

            source.RemoveOrganism();
            destination.AddOrganism(organism);
            this.OrganismHabitats[organism] = destination;
        }

        public void AddOrganism(Organism organism, Coordinates location)
        {
            var habitat = this.Habitats[location.X, location.Y];
            habitat.AddOrganism(organism);
            this.OrganismHabitats.Add(organism, habitat);
        }

        public void RemoveOrganism(Organism organism)
        {
            var habitat = this.OrganismHabitats[organism];
            habitat.RemoveOrganism();
            this.OrganismHabitats.Remove(organism);
        }

        public void SetTerrain(Coordinates location, Terrain terrain)
        {
            this.Habitats[location.X, location.Y].Environment.SetTerrain(terrain);
        }

        private List<Habitat> GetNeighbouringHabitats(Habitat habitat)
        {
            var neighbouringHabitats = new List<Habitat>();

            var location = this.HabitatCoordinates[habitat];
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

                    // do not carry on if (x, y) is diagonal from organism
                    if (x != location.X && y != location.Y)
                    {
                        continue;
                    }

                    neighbouringHabitats.Add(this.Habitats[x, y]);
                }
            }

            return neighbouringHabitats;
        }

        public void SetMeasureBias(Measure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismHabitats.Count);
        }
    }
}