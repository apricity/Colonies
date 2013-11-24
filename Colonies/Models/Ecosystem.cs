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
            var previousOrganismCoordinates = this.ecosystemData.GetOrganisms()
                .ToDictionary(organism => organism, organism => this.ecosystemData.CoordinateOf(organism));

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

            var currentOrganismCoordinates = this.ecosystemData.GetOrganisms()
                .ToDictionary(organism => organism, organism => this.ecosystemData.CoordinateOf(organism));

            return new UpdateSummary(previousOrganismCoordinates, currentOrganismCoordinates, alteredEnvironmentCoordinates.Distinct().ToList());
        }

        private IEnumerable<Coordinate> PerformPreMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();
            alteredEnvironmentCoordinates.AddRange(this.OrganismsConsumeNutrients());
            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformMovements()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var desiredOrganismCoordinates = EcosystemLogic.GetDesiredOrganismHabitats(this.ecosystemData);
            var movedOrganismCoordinates = EcosystemLogic.ResolveOrganismHabitats(this.ecosystemData, desiredOrganismCoordinates, new List<Organism>(), this);

            alteredEnvironmentCoordinates.AddRange(this.IncreasePheromoneLevels());
            alteredEnvironmentCoordinates.AddRange(this.IncreaseMineralLevels());

            foreach (var movedOrganismCoordinate in movedOrganismCoordinates)
            {
                this.ecosystemData.MoveOrganism(movedOrganismCoordinate.Key, movedOrganismCoordinate.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.ecosystemData.HasMeasure(coordinate, Measure.Obstruction));
            foreach (var obstructedCoordinate in obstructedCoordinates)
            {
                if (this.ecosystemData.DecreaseLevel(obstructedCoordinate, Measure.Obstruction, this.ObstructionDemolishRate))
                {
                    alteredEnvironmentCoordinates.Add(obstructedCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformPostMovementActions(Dictionary<Organism, Coordinate> previousOrganismCoordinates)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            alteredEnvironmentCoordinates.AddRange(this.DecreasePheromoneLevel());
            alteredEnvironmentCoordinates.AddRange(this.IncreaseNutrientLevels());
            alteredEnvironmentCoordinates.AddRange(this.SpreadHazards());
            alteredEnvironmentCoordinates.AddRange(this.DecreaseOrganismHealth());

            return alteredEnvironmentCoordinates.Distinct();
        }

        private IEnumerable<Coordinate> OrganismsConsumeNutrients()
        {
            var organismCoordinates = this.ecosystemData.OrganismCoordinates(true, null)
                .Where(coordinate => this.ecosystemData.HasMeasure(coordinate, Measure.Nutrient)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var organismHealth = this.ecosystemData.GetLevel(organismCoordinate, Measure.Health);
                var habitatNutrient = this.ecosystemData.GetLevel(organismCoordinate, Measure.Nutrient);
                var desiredNutrient = 1 - organismHealth;
                var nutrientTaken = Math.Min(desiredNutrient, habitatNutrient); 

                var healthIncreased = this.ecosystemData.IncreaseLevel(organismCoordinate, Measure.Health, nutrientTaken);
                var nutrientDecreased = this.ecosystemData.DecreaseLevel(organismCoordinate, Measure.Nutrient, nutrientTaken);
                if (healthIncreased || nutrientDecreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> DecreasePheromoneLevel()
        {
            var pheromoneCoordinates = this.ecosystemData.AllCoordinates()
                .Where(coordinate => this.ecosystemData.HasMeasure(coordinate, Measure.Pheromone)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var pheromoneCoordinate in pheromoneCoordinates)
            {
                var pheromoneDecreased = this.ecosystemData.DecreaseLevel(pheromoneCoordinate, Measure.Pheromone, this.PheromoneFadeRate);
                if (pheromoneDecreased)
                {
                    alteredEnvironmentCoordinates.Add(pheromoneCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreasePheromoneLevels()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var organismCoordinates = this.ecosystemData.OrganismCoordinates(true, true).ToList();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var pheromoneIncreased = this.ecosystemData.IncreaseLevel(organismCoordinate, Measure.Pheromone, this.PheromoneDepositRate);
                if (pheromoneIncreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseMineralLevels()
        {
            // only increase mineral where the terrain is not hazardous (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var organismCoordinates = this.ecosystemData.OrganismCoordinates(null, null)
                .Where(coordinate => !this.ecosystemData.IsHazardous(coordinate)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var mineralIncreased = this.ecosystemData.IncreaseLevel(organismCoordinate, Measure.Mineral, this.MineralFormRate);
                if (mineralIncreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseNutrientLevels()
        {
            var nutrientCoordinates = this.ecosystemData.AllCoordinates()
                .Where(coordinate => this.ecosystemData.HasMeasure(coordinate, Measure.Nutrient) 
                                     && !this.ecosystemData.IsHazardous(coordinate)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var nutrientCoordinate in nutrientCoordinates)
            {
                var nutrientIncreased = this.ecosystemData.IncreaseLevel(nutrientCoordinate, Measure.Nutrient, this.NutrientGrowthRate);
                if (nutrientIncreased)
                {
                    alteredEnvironmentCoordinates.Add(nutrientCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> DecreaseOrganismHealth()
        {
            var organismCoordinates = this.ecosystemData.OrganismCoordinates(true, null).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var healthDecreased = this.ecosystemData.DecreaseLevel(organismCoordinate, Measure.Health, this.HealthDeteriorationRate);
                if (healthDecreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> SpreadHazards()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var hazardMeasure in Environment.HazardMeasures())
            {
                var hazardCoordinates = this.ecosystemData.GetHazardCoordinates(hazardMeasure).ToList();
                foreach (var hazardCoordinate in hazardCoordinates)
                {
                    if (!DecisionLogic.IsSuccessful(this.HazardSpreadRate))
                    {
                        continue;
                    }

                    var neighbouringCoordinates = this.ecosystemData.GetNeighbours(hazardCoordinate, 1, false, false).ToList();
                    var validNeighbouringCoordinates = neighbouringCoordinates.Where(neighbourCoordinate =>
                        neighbourCoordinate != null 
                        && !this.ecosystemData.HasMeasure(neighbourCoordinate, Measure.Obstruction)
                        && this.ecosystemData.GetLevel(neighbourCoordinate, hazardMeasure) < 1).ToList();
                    if (validNeighbouringCoordinates.Count == 0)
                    {
                        continue;
                    }

                    var chosenCoordinate = DecisionLogic.MakeDecision(validNeighbouringCoordinates);
                    alteredEnvironmentCoordinates.AddRange(this.InsertHazard(chosenCoordinate, hazardMeasure));
                }
            }

            return alteredEnvironmentCoordinates;
        }

        public IEnumerable<Coordinate> InsertHazard(Coordinate coordinate, Measure measure)
        {
            if (!Environment.IsPotentialHazard(measure))
            {
                throw new InvalidEnumArgumentException(string.Format("{0} is not a potential hazard", measure));
            }

            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var neighbouringCoordinates = this.ecosystemData.GetNeighbours(coordinate, this.HazardRadius, true, true);
            var gaussianKernel = new GaussianBlur(0.25 * this.HazardDiameter, this.HazardDiameter).Kernel;

            var gaussianCentre = (double)gaussianKernel[this.HazardRadius, this.HazardRadius];
            for (var x = 0; x < this.HazardDiameter; x++)
            {
                for (var y = 0; y < this.HazardDiameter; y++)
                {
                    var level = gaussianKernel[x, y] / gaussianCentre;
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];

                    if (neighbouringCoordinate != null && level > this.ecosystemData.GetLevel(neighbouringCoordinate, measure))
                    {
                        this.ecosystemData.SetLevel(neighbouringCoordinate, measure, level);
                        alteredEnvironmentCoordinates.Add(neighbouringCoordinate);
                    }
                }
            }

            this.ecosystemData.InsertHazard(measure, coordinate);

            return alteredEnvironmentCoordinates;
        }

        public void SetLevel(Coordinate coordinate, Measure measure, double level)
        {
            this.ecosystemData.SetLevel(coordinate, measure, level);
        }

        public void SetMeasureBias(Measure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.ecosystemData.GetOrganisms().Count());
        }
    }
}