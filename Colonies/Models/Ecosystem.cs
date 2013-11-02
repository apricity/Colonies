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
        public Dictionary<Habitat, Coordinate> HabitatCoordinates { get; private set; } 
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

            this.HabitatCoordinates = new Dictionary<Habitat, Coordinate>();
            for (var i = 0; i < this.Width; i++)
            {
                for (var j = 0; j < this.Height; j++)
                {
                    this.HabitatCoordinates.Add(this.Habitats[i, j], new Coordinate(i, j));
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
            var previousOrganismCoordinates = this.OrganismHabitats.ToDictionary(
                organismCoordinate => organismCoordinate.Key, 
                organismCoordinate => this.HabitatCoordinates[organismCoordinate.Value]);

            var alteredEnvironmentCoordinates = new List<Coordinate>();

            /* perform pre movement actions e.g. take food, eat food, attack */
            alteredEnvironmentCoordinates.AddRange(this.PerformPreMovementActions());

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to 
             * and to resolve any conflicting intentions */
            alteredEnvironmentCoordinates.AddRange(this.PerformMovements());

            /* reduce pheromone level in all environments
             * increase pheromone, mineral, and nutrient where appropriate 
             * and demolish obstruction if they have blocked a movement */
            alteredEnvironmentCoordinates.AddRange(this.PerformPostMovementActions(previousOrganismCoordinates));

            var currentOrganismCoordinates = this.OrganismHabitats.ToDictionary(
                organismCoordinate => organismCoordinate.Key,
                organismCoordinate => this.HabitatCoordinates[organismCoordinate.Value]);

            return new UpdateSummary(previousOrganismCoordinates, currentOrganismCoordinates, alteredEnvironmentCoordinates.Distinct().ToList());
        }

        private IEnumerable<Coordinate> PerformPreMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

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
                    if (habitat.Environment.DecreaseLevel(Measure.Nutrient, nutrientTaken))
                    {
                        alteredEnvironmentCoordinates.Add(this.HabitatCoordinates[habitat]);
                    }
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformMovements()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var desiredOrganismHabitats = this.GetDesiredOrganismHabitats();
            var movedOrganismHabitats = this.ResolveOrganismHabitats(desiredOrganismHabitats, new List<Organism>());
            foreach (var movedOrganismHabitat in movedOrganismHabitats)
            {
                this.MoveOrganism(movedOrganismHabitat.Key, movedOrganismHabitat.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            foreach (var obstructedHabitat in desiredOrganismHabitats.Values.Where(habitat => habitat.IsObstructed()))
            {
                if (obstructedHabitat.Environment.DecreaseLevel(Measure.Obstruction, this.ObstructionDemolishRate))
                {
                    alteredEnvironmentCoordinates.Add(this.HabitatCoordinates[obstructedHabitat]);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformPostMovementActions(Dictionary<Organism, Coordinate> previousOrganismCoordinates)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            alteredEnvironmentCoordinates.AddRange(this.DecreasePheromoneLevel());
            alteredEnvironmentCoordinates.AddRange(this.IncreasePheromoneLevels(previousOrganismCoordinates));
            alteredEnvironmentCoordinates.AddRange(this.IncreaseMineralLevels(previousOrganismCoordinates));
            alteredEnvironmentCoordinates.AddRange(this.IncreaseNutrientLevels());
            this.DecreaseOrganismHealth();

            return alteredEnvironmentCoordinates.Distinct();
        }

        protected virtual Dictionary<Organism, Habitat> GetDesiredOrganismHabitats()
        {
            var desiredOrganismHabitats = new Dictionary<Organism, Habitat>();
            var aliveOrganismHabitats = this.OrganismHabitats.Where(organismHabitats => organismHabitats.Key.IsAlive).ToList();
            foreach (var organismHabitat in aliveOrganismHabitats)
            {
                var currentOrganism = organismHabitat.Key;
                var currentHabitat = organismHabitat.Value;

                // get measurements of neighbouring environments
                var neighbouringHabitats = this.GetNeighbouringHabitats(currentHabitat, 1, false).ToList();
                var validNeighbouringHabitats = neighbouringHabitats.Where(habitat => habitat != null).ToList();
                var neighbouringEnvironments = validNeighbouringHabitats.Select(neighbour => neighbour.Environment).ToList();

                // determine organism's intentions based on the environment measurements
                var chosenEnvironment = DecisionLogic.MakeDecision(neighbouringEnvironments, currentOrganism);

                // get the habitat the environment is from - this is where the organism wants to move to
                var chosenHabitat = validNeighbouringHabitats.Single(habitat => habitat.Environment.Equals(chosenEnvironment));
                desiredOrganismHabitats.Add(currentOrganism, chosenHabitat);
            }

            return desiredOrganismHabitats;
        }

        private Dictionary<Organism, Habitat> ResolveOrganismHabitats(Dictionary<Organism, Habitat> desiredOrganismHabitats, IEnumerable<Organism> alreadyResolvedOrganisms)
        {
            var resolvedOrganismHabitats = new Dictionary<Organism, Habitat>();

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
            var desiredHabitats = desiredOrganismHabitats.Values.ToList();

            // if there are no vacant habitats, this is our base case
            // return an empty list - i.e. no organism can move to its intended destination
            var vacantHabitats = desiredHabitats.Except(occupiedHabitats).Where(habitat => !habitat.IsObstructed()).ToList();
            if (vacantHabitats.Count == 0)
            {
                return resolvedOrganismHabitats;
            }

            foreach (var habitat in vacantHabitats)
            {
                // do not want LINQ expression to have foreach variable access, so copy to local variable
                var vacantHabitat = habitat;
                var conflictingOrganisms = desiredOrganismHabitats
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
                        desiredOrganismHabitats[remainingOrganism] = this.OrganismHabitats[remainingOrganism];
                    }
                }
                else
                {
                    organismToMove = conflictingOrganisms.Single();
                }

                // intended movement becomes an actual, resolved movement
                resolvedOrganismHabitats.Add(organismToMove, desiredOrganismHabitats[organismToMove]);
                desiredOrganismHabitats.Remove(organismToMove);
            }

            // need to recursively call resolve organism destinations with the knowledge of what has been resolved so far
            // so those resolved can be taken into consideration when calculating which destinations are now vacant
            var resolvedOrganisms = resolvedOrganismHabitats.Keys.ToList();
            var trailingOrganismHabitats = this.ResolveOrganismHabitats(desiredOrganismHabitats, resolvedOrganisms);
            foreach (var trailingOrganismHabitat in trailingOrganismHabitats)
            {
                resolvedOrganismHabitats.Add(trailingOrganismHabitat.Key, trailingOrganismHabitat.Value);
            }

            return resolvedOrganismHabitats;
        }

        protected virtual Organism DecideOrganism(List<Organism> organisms)
        {
            // this is in a virtual method so the mock ecosystem can override for testing
            return DecisionLogic.MakeDecision(organisms, this);
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

        private IEnumerable<Coordinate> DecreasePheromoneLevel()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var habitat in this.Habitats)
            {
                if (habitat.Environment.GetLevel(Measure.Pheromone).Equals(0.0))
                {
                    continue;
                }

                if (habitat.Environment.DecreaseLevel(Measure.Pheromone, this.PheromoneFadeRate))
                {
                    alteredEnvironmentCoordinates.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreasePheromoneLevels(Dictionary<Organism, Coordinate> organismCoordinates)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            // only increase pheromones where the organism is alive and is depositing pheromones
            var validCoordinates = organismCoordinates.Where(pair => pair.Key.IsAlive && pair.Key.IsDepositingPheromones)
                                                  .ToDictionary(pair => pair.Key, pair => pair.Value)
                                                  .Values.ToList();

            foreach (var location in validCoordinates)
            {
                var habitat = this.Habitats[location.X, location.Y];
                if (habitat.Environment.IncreaseLevel(Measure.Pheromone, this.PheromoneDepositRate))
                {
                    alteredEnvironmentCoordinates.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseMineralLevels(Dictionary<Organism, Coordinate> organismCoordinates)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            // only increase mineral where the terrain is earth (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var validCoordinates = organismCoordinates.Values.ToList();
            foreach (var location in validCoordinates)
            {
                var habitat = this.Habitats[location.X, location.Y];
                if (!habitat.Environment.Terrain.Equals(Terrain.Earth))
                {
                    continue;
                }

                if (habitat.Environment.IncreaseLevel(Measure.Mineral, this.MineralFormRate))
                {
                    alteredEnvironmentCoordinates.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseNutrientLevels()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var habitat in this.Habitats)
            {
                if (!habitat.Environment.HasNutrient)
                {
                    continue;
                }

                if (habitat.Environment.IncreaseLevel(Measure.Nutrient, this.NutrientGrowthRate))
                {
                    alteredEnvironmentCoordinates.Add(this.HabitatCoordinates[habitat]);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private void DecreaseOrganismHealth()
        {
            foreach (var organism in this.OrganismHabitats.Keys.ToList())
            {
                organism.DecreaseLevel(Measure.Health, this.HealthDeteriorationRate);
            }
        }

        public void AddOrganism(Organism organism, Coordinate location)
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

        public void Insert(Terrain terrain, Measure measure, Coordinate coordinates)
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