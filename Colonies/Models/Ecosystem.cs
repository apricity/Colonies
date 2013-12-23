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

    public class Ecosystem : IEcosystem
    {
        private EcosystemData EcosystemData { get; set; }
        public IWeather Weather { get; private set; }

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
                return this.EcosystemData.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.EcosystemData.Height;
            }
        }

        public Ecosystem(EcosystemData ecosystemData, IWeather weather)
        {
            this.EcosystemData = ecosystemData;
            this.Weather = weather;

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
            var previousOrganismCoordinates = this.EcosystemData.GetOrganismCoordinates(null, null)
                .ToDictionary(coordinate => this.EcosystemData.GetOrganism(coordinate), coordinate => coordinate);

            var alteredEnvironmentCoordinates = new List<Coordinate>();

            /* perform pre movement actions (e.g. take food, eat food, attack) */
            alteredEnvironmentCoordinates.AddRange(this.PerformPreMovementActions());

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to and to resolve any conflicting intentions 
             * and change measures related to movement (e.g. pheromone, mineral, obstruction) */
            alteredEnvironmentCoordinates.AddRange(this.PerformMovementsActions());

            /* change measures that are globally affected (e.g. nutrient growth, pheromone fade, hazard spread, health deterioration */
            alteredEnvironmentCoordinates.AddRange(this.PerformPostMovementActions());

            var currentOrganismCoordinates = this.EcosystemData.GetOrganismCoordinates(null, null)
                .ToDictionary(coordinate => this.EcosystemData.GetOrganism(coordinate), coordinate => coordinate);

            return new UpdateSummary(previousOrganismCoordinates, currentOrganismCoordinates, alteredEnvironmentCoordinates.Distinct().ToList());
        }

        private IEnumerable<Coordinate> PerformPreMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();
            alteredEnvironmentCoordinates.AddRange(this.OrganismsConsumeNutrients());
            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformMovementsActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var desiredOrganismCoordinates = EcosystemLogic.GetDesiredCoordinates(this.EcosystemData);
            var movedOrganismCoordinates = EcosystemLogic.ResolveOrganismHabitats(this.EcosystemData, desiredOrganismCoordinates, new List<IOrganism>(), this);

            alteredEnvironmentCoordinates.AddRange(this.IncreasePheromoneLevels());
            alteredEnvironmentCoordinates.AddRange(this.IncreaseMineralLevels());

            foreach (var movedOrganismCoordinate in movedOrganismCoordinates)
            {
                this.EcosystemData.MoveOrganism(movedOrganismCoordinate.Key, movedOrganismCoordinate.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.EcosystemData.HasEnvironmentMeasure(coordinate, Measure.Obstruction));
            foreach (var obstructedCoordinate in obstructedCoordinates)
            {
                var obstructionDecreased = this.EcosystemData.DecreaseEnvironmentLevel(obstructedCoordinate, Measure.Obstruction, this.ObstructionDemolishRate);
                if (obstructionDecreased)
                {
                    alteredEnvironmentCoordinates.Add(obstructedCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformPostMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            alteredEnvironmentCoordinates.AddRange(this.DecreasePheromoneLevel());
            alteredEnvironmentCoordinates.AddRange(this.IncreaseNutrientLevels());
            alteredEnvironmentCoordinates.AddRange(this.ProgressWeather());
            //alteredEnvironmentCoordinates.AddRange(this.SpreadHazards());
            alteredEnvironmentCoordinates.AddRange(this.DecreaseOrganismHealth());

            return alteredEnvironmentCoordinates.Distinct();
        }

        private IEnumerable<Coordinate> OrganismsConsumeNutrients()
        {
            var organismCoordinates = this.EcosystemData.GetOrganismCoordinates(true, null)
                .Where(coordinate => this.EcosystemData.HasEnvironmentMeasure(coordinate, Measure.Nutrient)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var organismHealth = this.EcosystemData.GetOrganismLevel(organismCoordinate, Measure.Health);
                var habitatNutrient = this.EcosystemData.GetEnvironmentLevel(organismCoordinate, Measure.Nutrient);
                var desiredNutrient = 1 - organismHealth;
                var nutrientTaken = Math.Min(desiredNutrient, habitatNutrient); 

                var healthIncreased = this.EcosystemData.IncreaseOrganismLevel(organismCoordinate, Measure.Health, nutrientTaken);
                var nutrientDecreased = this.EcosystemData.DecreaseEnvironmentLevel(organismCoordinate, Measure.Nutrient, nutrientTaken);
                if (healthIncreased || nutrientDecreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> DecreasePheromoneLevel()
        {
            var pheromoneCoordinates = this.EcosystemData.GetAllCoordinates()
                .Where(coordinate => this.EcosystemData.HasEnvironmentMeasure(coordinate, Measure.Pheromone)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var pheromoneCoordinate in pheromoneCoordinates)
            {
                var pheromoneDecreased = this.EcosystemData.DecreaseEnvironmentLevel(pheromoneCoordinate, Measure.Pheromone, this.PheromoneFadeRate);
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

            var organismCoordinates = this.EcosystemData.GetOrganismCoordinates(true, true).ToList();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var pheromoneIncreased = this.EcosystemData.IncreaseEnvironmentLevel(organismCoordinate, Measure.Pheromone, this.PheromoneDepositRate);
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
            var organismCoordinates = this.EcosystemData.GetOrganismCoordinates(null, null)
                .Where(coordinate => !this.EcosystemData.IsHazardous(coordinate)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var mineralIncreased = this.EcosystemData.IncreaseEnvironmentLevel(organismCoordinate, Measure.Mineral, this.MineralFormRate);
                if (mineralIncreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseNutrientLevels()
        {
            var nutrientCoordinates = this.EcosystemData.GetAllCoordinates()
                .Where(coordinate => this.EcosystemData.HasEnvironmentMeasure(coordinate, Measure.Nutrient) 
                                     && !this.EcosystemData.IsHazardous(coordinate)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var nutrientCoordinate in nutrientCoordinates)
            {
                var nutrientIncreased = this.EcosystemData.IncreaseEnvironmentLevel(nutrientCoordinate, Measure.Nutrient, this.NutrientGrowthRate);
                if (nutrientIncreased)
                {
                    alteredEnvironmentCoordinates.Add(nutrientCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> DecreaseOrganismHealth()
        {
            var organismCoordinates = this.EcosystemData.GetOrganismCoordinates(true, null).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var healthDecreased = this.EcosystemData.DecreaseOrganismLevel(organismCoordinate, Measure.Health, this.HealthDeteriorationRate);
                if (healthDecreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> ProgressWeather()
        {
            this.Weather.Progress();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var weatherType in this.Weather.WeatherTypes)
            {
                var spreadChance = this.HazardSpreadRate * this.Weather.GetWeatherLevel(weatherType);
                alteredEnvironmentCoordinates.AddRange(this.SpreadHazards(this.Weather.GetWeatherHazard(weatherType), spreadChance));
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> SpreadHazards(Measure hazardMeasure, double spreadChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(hazardMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!DecisionLogic.IsSuccessful(spreadChance))
                {
                    continue;
                }

                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(hazardCoordinate, 1, false, false).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(neighbourCoordinate =>
                    neighbourCoordinate != null
                    && !this.EcosystemData.HasEnvironmentMeasure(neighbourCoordinate, Measure.Obstruction)
                    && this.EcosystemData.GetEnvironmentLevel(neighbourCoordinate, hazardMeasure) < 1).ToList();
                if (validNeighbouringCoordinates.Count == 0)
                {
                    continue;
                }

                var chosenCoordinate = DecisionLogic.MakeDecision(validNeighbouringCoordinates);
                alteredEnvironmentCoordinates.AddRange(this.InsertHazard(chosenCoordinate, hazardMeasure));
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

            var neighbouringCoordinates = this.EcosystemData.GetNeighbours(coordinate, this.HazardRadius, true, true);
            var gaussianKernel = new GaussianBlur(0.25 * this.HazardDiameter, this.HazardDiameter).Kernel;

            var gaussianCentre = (double)gaussianKernel[this.HazardRadius, this.HazardRadius];
            for (var x = 0; x < this.HazardDiameter; x++)
            {
                for (var y = 0; y < this.HazardDiameter; y++)
                {
                    var level = gaussianKernel[x, y] / gaussianCentre;
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];

                    if (neighbouringCoordinate != null 
                        && level > this.EcosystemData.GetEnvironmentLevel(neighbouringCoordinate, measure))
                    {
                        this.EcosystemData.SetEnvironmentLevel(neighbouringCoordinate, measure, level);
                        alteredEnvironmentCoordinates.Add(neighbouringCoordinate);
                    }
                }
            }

            this.EcosystemData.InsertHazard(measure, coordinate);

            return alteredEnvironmentCoordinates;
        }

        public void SetEnvironmentLevel(Coordinate coordinate, Measure measure, double level)
        {
            this.EcosystemData.SetEnvironmentLevel(coordinate, measure, level);
        }

        public void SetMeasureBias(Measure measure, double bias)
        {
            this.MeasureBiases[measure] = bias;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.EcosystemData.GetOrganismCoordinates(null, null).Count());
        }
    }
}