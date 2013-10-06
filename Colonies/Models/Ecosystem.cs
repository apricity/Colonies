namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AForge.Imaging.Filters;

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
        public double MineralFormRate { get; set; }
        public double ObstructionDemolishRate { get; set; }

        private int ConditionSpreadDiameter { get; set; }
        private int ConditionSpreadRadius
        {
            get
            {
                return (this.ConditionSpreadDiameter - 1) / 2;
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
            this.MineralFormRate = 1 / 100.0;
            this.ObstructionDemolishRate = 1 / 5.0;

            // work out how big any fire/water spread should be based on ecosystem dimensions
            this.ConditionSpreadDiameter = CalculateConditionSpreadDiameter();
        }

        public UpdateSummary Update()
        {
            /* record the pre-update locations */
            var preUpdateOrganismLocations = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key, 
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);

            /* perform in-situ actions e.g. take food, eat food, attack */
            foreach (var organismHabitat in this.OrganismHabitats)
            {
                var organism = organismHabitat.Key;
                var habitat = organismHabitat.Value;
                var habitatNutrientLevel = habitat.Environment.GetLevel(Measure.Nutrient);

                if (habitatNutrientLevel > 0.0)
                {
                    var desiredNutrients = 1 - organism.GetLevel(Measure.Health);
                    var nutrientTaken = Math.Min(desiredNutrients, habitatNutrientLevel);

                    organism.IncreaseLevel(Measure.Health, nutrientTaken);
                    habitat.Environment.DecreaseLevel(Measure.Nutrient, nutrientTaken);
                }
            }

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to 
             * and to resolve any conflicting intentions */
            var intendedOrganismDestinations = this.GetIntendedOrganismDestinations();
            var actualOrganismDestinations = this.ResolveOrganismDestinations(intendedOrganismDestinations, new List<Organism>());

            /* perform ex-situ actions e.g. move any organisms that can after resolving conflicting intentions */
            foreach (var actualOrganismDestination in actualOrganismDestinations)
            {
                this.MoveOrganism(actualOrganismDestination.Key, actualOrganismDestination.Value);                    
            }

            /* reduce pheromone level in all environments
             * increase pheromone, mineral, and nutrient where appropriate 
             * and demolish obstruction if they have blocked a movement */
            var pheromoneDecreasedLocations = this.DecreasePheromoneLevel();
            this.IncreasePheromoneLevels(preUpdateOrganismLocations);
            this.IncreaseMineralLevels(preUpdateOrganismLocations);
            var nutrientGrowthLocations = this.IncreaseNutrientLevels();
            var obstructionDemolishLocations = new List<Coordinates>();
            foreach (var obstructedHabitat in intendedOrganismDestinations.Values.Where(habitat => habitat.IsObstructed()))
            {
                obstructedHabitat.Environment.DecreaseLevel(Measure.Obstruction, this.ObstructionDemolishRate);
                obstructionDemolishLocations.Add(this.HabitatCoordinates[obstructedHabitat]);
            }

            /* reduce all organisms health */
            this.DecreaseOrganismHealth();

            /* record the post-update locations */
            var postUpdateOrganismLocations = this.OrganismHabitats.ToDictionary(
                organismHabitat => organismHabitat.Key.ToString(), 
                organismHabitat => this.HabitatCoordinates[organismHabitat.Value]);

            return new UpdateSummary(preUpdateOrganismLocations.Values.ToList(), postUpdateOrganismLocations.Values.ToList(), pheromoneDecreasedLocations, nutrientGrowthLocations, obstructionDemolishLocations);
        }

        private List<Coordinates> DecreasePheromoneLevel()
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

        private void IncreasePheromoneLevels(Dictionary<Organism, Coordinates> organismLocations)
        {
            // only increase pheromones where the organism is alive and is depositing pheromones
            var validLocations = organismLocations.Where(pair => pair.Key.IsAlive && pair.Key.IsDepositingPheromones)
                                                  .ToDictionary(pair => pair.Key, pair => pair.Value)
                                                  .Values.ToList();

            foreach (var location in validLocations)
            {
                var habitat = this.Habitats[location.X, location.Y];
                habitat.Environment.IncreaseLevel(Measure.Pheromone, this.PheromoneDepositRate);
            }
        }

        private void IncreaseMineralLevels(Dictionary<Organism, Coordinates> organismLocations)
        {
            // only increase mineral where the terrain is earth (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var validLocations = organismLocations.Values.ToList();
            foreach (var location in validLocations)
            {
                var habitat = this.Habitats[location.X, location.Y];
                if (!habitat.Environment.Terrain.Equals(Terrain.Earth))
                {
                    continue;
                }

                habitat.Environment.IncreaseLevel(Measure.Mineral, this.MineralFormRate);
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

        private void DecreaseOrganismHealth()
        {
            foreach (var organism in this.OrganismHabitats.Keys.ToList())
            {
                organism.DecreaseLevel(Measure.Health, this.HealthDeteriorationRate);
            }
        }

        protected virtual Dictionary<Organism, Habitat> GetIntendedOrganismDestinations()
        {
            var intendedOrganismDestinations = new Dictionary<Organism, Habitat>();
            var aliveOrganismHabitats = this.OrganismHabitats.Where(organismHabitats => organismHabitats.Key.IsAlive).ToList();
            foreach (var organismCoordinates in aliveOrganismHabitats)
            {
                var currentOrganism = organismCoordinates.Key;
                var currentHabitat = organismCoordinates.Value;

                // get measurements of neighbouring environments
                var neighbouringHabitats = this.GetNeighbouringHabitats(currentHabitat, 1, false).ToList();
                var validNeighbouringHabitats = neighbouringHabitats.Where(habitat => habitat != null).ToList();
                var neighbouringEnvironments = validNeighbouringHabitats.Select(neighbour => neighbour.Environment).ToList();

                // determine organism's intentions based on the environment measurements
                var chosenEnvironment = DecisionLogic.MakeDecision(neighbouringEnvironments, currentOrganism);

                // get the habitat the environment is from - this is where the organism wants to move to
                var chosenHabitat = validNeighbouringHabitats.Single(habitat => habitat.Environment.Equals(chosenEnvironment));
                intendedOrganismDestinations.Add(currentOrganism, chosenHabitat);
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

        public void Insert(Terrain terrain, Measure measure, Coordinates coordinates)
        {
            var habitat = this.Habitats[coordinates.X, coordinates.Y];
            habitat.Environment.SetLevel(measure, 1.0);
            habitat.Environment.SetTerrain(terrain);

            var neighbouringHabitats = this.GetNeighbouringHabitats(habitat, this.ConditionSpreadRadius, true);
            var gaussianKernel = new GaussianBlur(0.25 * this.ConditionSpreadDiameter, this.ConditionSpreadDiameter).Kernel;

            var gaussianCentre = (double)gaussianKernel[this.ConditionSpreadRadius, this.ConditionSpreadRadius];
            for (var x = 0; x < this.ConditionSpreadDiameter; x++)
            {
                for (var y = 0; y < this.ConditionSpreadDiameter; y++)
                {
                    var level = gaussianKernel[x, y] / gaussianCentre;
                    var neighbouringHabitat = neighbouringHabitats[x, y];

                    if (neighbouringHabitat != null && level > neighbouringHabitat.Environment.GetLevel(measure))
                    {
                        neighbouringHabitat.Environment.SetLevel(measure, level);
                    }
                }
            }
        }

        private Habitat[,] GetNeighbouringHabitats(Habitat habitat, int neighbourDepth, bool includeDiagonals)
        {
            //var neighbouringHabitats = new List<Habitat>();
            var neighbouringHabitats = new Habitat[(neighbourDepth * 2) + 1, (neighbourDepth * 2) + 1];

            var location = this.HabitatCoordinates[habitat];
            for (var i = -neighbourDepth; i <= neighbourDepth; i++)
            {
                var x = i + location.X;

                // do not carry on if x is out-of-bounds
                if (x < 0 || x >= this.Width)
                {
                    continue;
                }

                for (var j = -neighbourDepth; j <= neighbourDepth; j++)
                {
                    var y = j + location.Y;

                    // do not carry on if y is out-of-bounds
                    if (y < 0 || y >= this.Height)
                    {
                        continue;
                    }

                    // do not carry on if (x, y) is diagonal from organism (and include diagonals is false)
                    if (x != location.X && y != location.Y && !includeDiagonals)
                    {
                        continue;
                    }

                    neighbouringHabitats[i + neighbourDepth, j + neighbourDepth] = this.Habitats[x, y];
                }
            }

            return neighbouringHabitats;
        }

        public void SetMeasureBias(Measure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        private int CalculateConditionSpreadDiameter()
        {
            var ecosystemArea = (double)(this.Height * this.Width);

            var diameterFound = false;
            var currentDiameter = 3; // minimum is 3x3
            while (!diameterFound)
            {
                var nextDiameter = currentDiameter + 2;
                if (Math.Pow(nextDiameter, 2) > Math.Sqrt(ecosystemArea))
                {
                    diameterFound = true;
                }
                else
                {
                    currentDiameter = nextDiameter;
                }
            }

            return currentDiameter;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.OrganismHabitats.Count);
        }
    }
}