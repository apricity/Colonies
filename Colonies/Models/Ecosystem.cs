namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using AForge.Imaging.Filters;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;
    using Wacton.Colonies.Logic;

    public class Ecosystem : IBiased
    {
        private readonly EcosystemData ecosystemData;

        public Dictionary<Measure, double> MeasureBiases { get; private set; }

        // TODO: neater management of these?
        public double HealthDeteriorationRate { get; set; }
        public double PheromoneDepositRate { get; set; }
        public double PheromoneFadeRate { get; set; }
        public double NutrientGrowthRate { get; set; }
        public double MineralFormRate { get; set; }
        public double ObstructionDemolishRate { get; set; }
        public double HazardSpreadRate { get; set; }

        private int HazardDiameter { get; set; }
        private int HazardRadius
        {
            get
            {
                return (this.HazardDiameter - 1) / 2;
            }
        }

        public int Width
        {
            get
            {
                return this.ecosystemData.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.ecosystemData.Height;
            }
        }

        public Ecosystem(EcosystemData ecosystemData)
        {
            this.ecosystemData = ecosystemData;
            this.MeasureBiases = new Dictionary<Measure, double> { { Measure.Health, 1 } };

            // work out how big any hazard spread should be based on ecosystem dimensions
            this.HazardDiameter = this.CalculateHazardDiameter();

            this.HealthDeteriorationRate = 1 / 500.0;
            this.PheromoneDepositRate = 1 / 100.0;
            this.PheromoneFadeRate = 1 / 500.0;
            this.NutrientGrowthRate = 1 / 500.0;
            this.MineralFormRate = 1 / 100.0;
            this.ObstructionDemolishRate = 1 / 5.0;
            this.HazardSpreadRate = 1 / 500.0;
        }

        public UpdateSummary Update()
        {
            var previousOrganismCoordinates = this.ecosystemData.GetOrganismCoordinates();

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

            var currentOrganismCoordinates = this.ecosystemData.GetOrganismCoordinates();

            return new UpdateSummary(previousOrganismCoordinates, currentOrganismCoordinates, alteredEnvironmentCoordinates.Distinct().ToList());
        }

        private IEnumerable<Coordinate> PerformPreMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var organism in this.ecosystemData.GetOrganisms())
            {
                var habitat = this.HabitatOf(organism);
                var habitatNutrientLevel = habitat.Environment.GetLevel(Measure.Nutrient);
                if (habitatNutrientLevel > 0.0)
                {
                    var desiredNutrients = 1 - organism.GetLevel(Measure.Health);
                    var nutrientTaken = Math.Min(desiredNutrients, habitatNutrientLevel);

                    organism.IncreaseLevel(Measure.Health, nutrientTaken);
                    if (habitat.Environment.DecreaseLevel(Measure.Nutrient, nutrientTaken))
                    {
                        alteredEnvironmentCoordinates.Add(this.ecosystemData.CoordinateOf(habitat));
                    }
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformMovements()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var desiredOrganismHabitats = EcosystemLogic.GetDesiredOrganismHabitats(this.ecosystemData);
            var movedOrganismHabitats = EcosystemLogic.ResolveOrganismHabitats(this.ecosystemData, desiredOrganismHabitats, new List<Organism>(), this);

            // TODO: move to ecosystem data?
            foreach (var movedOrganismHabitat in movedOrganismHabitats)
            {
                this.MoveOrganism(movedOrganismHabitat.Key, movedOrganismHabitat.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            foreach (var obstructedHabitat in desiredOrganismHabitats.Values.Where(habitat => habitat.IsObstructed()))
            {
                if (obstructedHabitat.Environment.DecreaseLevel(Measure.Obstruction, this.ObstructionDemolishRate))
                {
                    alteredEnvironmentCoordinates.Add(this.ecosystemData.CoordinateOf(obstructedHabitat));
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
            alteredEnvironmentCoordinates.AddRange(this.SpreadHazards());
            this.DecreaseOrganismHealth();

            return alteredEnvironmentCoordinates.Distinct();
        }

        // TODO: move to ecosystemData?
        private void MoveOrganism(Organism organism, Habitat destination)
        {
            var source = this.HabitatOf(organism);

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
            this.ecosystemData.OrganismHabitats[organism] = destination;
        }

        private IEnumerable<Coordinate> DecreasePheromoneLevel()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var habitat in this.ecosystemData.Habitats)
            {
                if (habitat.Environment.GetLevel(Measure.Pheromone).Equals(0.0))
                {
                    continue;
                }

                if (habitat.Environment.DecreaseLevel(Measure.Pheromone, this.PheromoneFadeRate))
                {
                    alteredEnvironmentCoordinates.Add(this.CoordinateOf(habitat));
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

            foreach (var coordinate in validCoordinates)
            {
                var habitat = this.HabitatAt(coordinate);
                if (habitat.Environment.IncreaseLevel(Measure.Pheromone, this.PheromoneDepositRate))
                {
                    alteredEnvironmentCoordinates.Add(this.CoordinateOf(habitat));
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseMineralLevels(Dictionary<Organism, Coordinate> organismCoordinates)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            // only increase mineral where the terrain is not hazardous (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var validCoordinates = organismCoordinates.Values.ToList();
            foreach (var coordinate in validCoordinates)
            {
                var habitat = this.HabitatAt(coordinate);
                if (habitat.Environment.IsHazardous)
                {
                    continue;
                }

                if (habitat.Environment.IncreaseLevel(Measure.Mineral, this.MineralFormRate))
                {
                    alteredEnvironmentCoordinates.Add(this.CoordinateOf(habitat));
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseNutrientLevels()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var habitat in this.ecosystemData.Habitats)
            {
                if (!habitat.Environment.HasNutrient || habitat.Environment.IsHazardous)
                {
                    continue;
                }

                if (habitat.Environment.IncreaseLevel(Measure.Nutrient, this.NutrientGrowthRate))
                {
                    alteredEnvironmentCoordinates.Add(this.CoordinateOf(habitat));
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> SpreadHazards()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            // cannot use this.CoordinateHazards directly, since inserting a hazard (which is inside the loop) will modify this.CoordinateHazards
            var coordinateHazards = this.ecosystemData.CoordinateHazards.ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var coordinateHazard in coordinateHazards)
            {
                foreach (var hazardMeasure in coordinateHazard.Value)
                {
                    if (!DecisionLogic.IsSuccessful(this.HazardSpreadRate))
                    {
                        continue;
                    }

                    var neighbouringHabitats = this.ecosystemData.Habitats.GetNeighbours(coordinateHazard.Key, 1, false, false).ToList();
                    var validNeighbouringHabitats = neighbouringHabitats.Where(habitat => habitat != null && !habitat.IsObstructed() && habitat.Environment.GetLevel(hazardMeasure) < 1).ToList();
                    if (validNeighbouringHabitats.Count == 0)
                    {
                        continue;
                    }
                    
                    var neighbouringCoordinates = validNeighbouringHabitats.Select(this.CoordinateOf).ToList();
                    var chosenCoordinate = DecisionLogic.MakeDecision(neighbouringCoordinates);
                    this.InsertHazard(hazardMeasure, chosenCoordinate);

                    var insertedNeighbouringHabitats = this.ecosystemData.Habitats.GetNeighbours(coordinateHazard.Key, this.HazardRadius, true, true).ToList();
                    var validInsertedNeighbouringHabitats = insertedNeighbouringHabitats.Where(habitat => habitat != null).ToList();

                    alteredEnvironmentCoordinates.AddRange(validInsertedNeighbouringHabitats.Select(this.CoordinateOf));
                } 
            }

            return alteredEnvironmentCoordinates;
        }

        private void DecreaseOrganismHealth()
        {
            foreach (var organism in this.ecosystemData.GetOrganisms())
            {
                organism.DecreaseLevel(Measure.Health, this.HealthDeteriorationRate);
            }
        }

        public void AddOrganism(Organism organism, Coordinate coordinate)
        {
            var habitat = this.HabitatAt(coordinate);
            habitat.AddOrganism(organism);
            this.ecosystemData.OrganismHabitats.Add(organism, habitat);
        }

        public void RemoveOrganism(Organism organism)
        {
            var habitat = this.ecosystemData.OrganismHabitats[organism];
            habitat.RemoveOrganism();
            this.ecosystemData.OrganismHabitats.Remove(organism);
        }

        public void InsertHazard(Measure measure, Coordinate coordinate)
        {
            if (!Environment.IsPotentialHazard(measure))
            {
                throw new InvalidEnumArgumentException(string.Format("{0} is not a potential hazard", measure.ToString()));
            }

            var neighbouringHabitats = this.ecosystemData.Habitats.GetNeighbours(coordinate, this.HazardRadius, true, true);
            var gaussianKernel = new GaussianBlur(0.25 * this.HazardDiameter, this.HazardDiameter).Kernel;

            var gaussianCentre = (double)gaussianKernel[this.HazardRadius, this.HazardRadius];
            for (var x = 0; x < this.HazardDiameter; x++)
            {
                for (var y = 0; y < this.HazardDiameter; y++)
                {
                    var level = gaussianKernel[x, y] / gaussianCentre;
                    var neighbouringHabitat = neighbouringHabitats[x, y];

                    if (neighbouringHabitat != null && level > neighbouringHabitat.Environment.GetLevel(measure))
                    {
                        neighbouringHabitat.Environment.SetLevel(measure, level);
                    }
                }
            }

            if (this.ecosystemData.CoordinateHazards.ContainsKey(coordinate))
            {
                this.ecosystemData.CoordinateHazards[coordinate].Add(measure);
            }
            else
            {
                this.ecosystemData.CoordinateHazards.Add(coordinate, new List<Measure> { measure });
            }
        }

        public Habitat HabitatAt(Coordinate coordinate)
        {
            return this.ecosystemData.HabitatAt(coordinate);
        }

        public Habitat HabitatOf(Organism organism)
        {
            return this.ecosystemData.HabitatOf(organism);
        }

        public Coordinate CoordinateOf(Habitat habitat)
        {
            return this.ecosystemData.CoordinateOf(habitat);
        }

        public Coordinate CoordinateOf(Organism organism)
        {
            return this.ecosystemData.CoordinateOf(organism);
        }

        public void SetMeasureBias(Measure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.ecosystemData.OrganismHabitats.Count);
        }
    }
}