namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AForge.Imaging.Filters;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Extensions;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.DataProviders;
    using Wacton.Colonies.Models.Interfaces;

    public class Ecosystem : IEcosystem
    {
        private EcosystemData EcosystemData { get; set; }
        public IWeather Weather { get; private set; }

        public Dictionary<OrganismMeasure, double> MeasureBiases { get; private set; }

        // TODO: neater management of these?
        public double HealthDeteriorationRate { get; set; }
        public double PheromoneDepositRate { get; set; }
        public double PheromoneFadeRate { get; set; }
        public double NutrientGrowthRate { get; set; }
        public double MineralFormRate { get; set; }
        public double ObstructionDemolishRate { get; set; }

        public Dictionary<EnvironmentMeasure, HazardRate> EnvironmentMeasureHazardRates { get; private set; }
        public Dictionary<EnvironmentMeasure, int> EnvironmentMeasureDiameters { get; private set; } 

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

            this.MeasureBiases = new Dictionary<OrganismMeasure, double> { { OrganismMeasure.Health, 1 } };

            // work out how big any hazard spread should be based on ecosystem dimensions
            var diameter = this.CalculateHazardDiameter();
            this.EnvironmentMeasureDiameters = new Dictionary<EnvironmentMeasure, int>
                {
                    { EnvironmentMeasure.Damp, diameter },
                    { EnvironmentMeasure.Heat, diameter },
                    { EnvironmentMeasure.Poison, diameter },
                    { EnvironmentMeasure.Sound, (diameter * 4) - 1 }
                };

            this.HealthDeteriorationRate = 1 / 500.0;
            this.PheromoneDepositRate = 1 / 100.0;
            this.PheromoneFadeRate = 1 / 500.0;
            this.NutrientGrowthRate = 1 / 500.0;
            this.MineralFormRate = 1 / 100.0;
            this.ObstructionDemolishRate = 1 / 5.0;

            this.EnvironmentMeasureHazardRates = new Dictionary<EnvironmentMeasure, HazardRate>
                {
                    { EnvironmentMeasure.Damp, new HazardRate(1 / 2000.0, 1 / 500.0, 1 / 1000.0) },
                    { EnvironmentMeasure.Heat, new HazardRate(1 / 2000.0, 1 / 500.0, 1 / 1000.0) },
                    { EnvironmentMeasure.Poison, new HazardRate(0.0, 1 / 50.0, 1 / 50.0) }
                };
        }

        public UpdateSummary Update()
        {
            var previousOrganismCoordinates = this.EcosystemData.OrganismCoordinates()
                .ToDictionary(coordinate => this.EcosystemData.GetOrganism(coordinate), coordinate => coordinate);

            var alteredEnvironmentCoordinates = new List<Coordinate>();

            /* perform pre movement actions (e.g. take food, eat food, attack) */
            alteredEnvironmentCoordinates.AddRange(this.PerformPreMovementActions());

            /* find out where each organism would like to move to
             * then analyse them to decide where the organisms will actually move to and to resolve any conflicting intentions 
             * and change measures related to movement (e.g. pheromone, mineral, obstruction) */
            alteredEnvironmentCoordinates.AddRange(this.PerformMovementsActions());

            /* change measures that are globally affected (e.g. nutrient growth, pheromone fade, hazard spread, health deterioration) */
            alteredEnvironmentCoordinates.AddRange(this.PerformPostMovementActions());

            var currentOrganismCoordinates = this.EcosystemData.OrganismCoordinates()
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

            var existingSoundCoordinates = this.EcosystemData.EmittingSoundOrganismCoordinates().ToList();

            var desiredOrganismCoordinates = EcosystemLogic.GetDesiredCoordinates(this.EcosystemData);
            var movedOrganismCoordinates = EcosystemLogic.ResolveOrganismHabitats(this.EcosystemData, desiredOrganismCoordinates, new List<IOrganism>(), this);

            alteredEnvironmentCoordinates.AddRange(this.IncreasePheromoneLevels());
            alteredEnvironmentCoordinates.AddRange(this.IncreaseMineralLevels());

            foreach (var movedOrganismCoordinate in movedOrganismCoordinates)
            {
                this.EcosystemData.MoveOrganism(movedOrganismCoordinate.Key, movedOrganismCoordinate.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction));
            foreach (var obstructedCoordinate in obstructedCoordinates)
            {
                var obstructionDecreased = this.EcosystemData.DecreaseLevel(obstructedCoordinate, EnvironmentMeasure.Obstruction, this.ObstructionDemolishRate);
                if (obstructionDecreased)
                {
                    alteredEnvironmentCoordinates.Add(obstructedCoordinate);
                }
            }

            foreach (var existingSoundCoordinate in existingSoundCoordinates)
            {
                alteredEnvironmentCoordinates.AddRange(this.RemoveDistributedMeasure(existingSoundCoordinate, EnvironmentMeasure.Sound));
            }

            var updatedSoundCoordinates = this.EcosystemData.EmittingSoundOrganismCoordinates().ToList();
            foreach (var updatedSoundCoordinate in updatedSoundCoordinates)
            {
                alteredEnvironmentCoordinates.AddRange(this.InsertDistributedMeasure(updatedSoundCoordinate, EnvironmentMeasure.Sound));
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformPostMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            alteredEnvironmentCoordinates.AddRange(this.DecreasePheromoneLevel());
            alteredEnvironmentCoordinates.AddRange(this.IncreaseNutrientLevels());
            alteredEnvironmentCoordinates.AddRange(this.ProgressWeatherAndHazards());

            var decreasedHealthCoordinates = this.DecreaseOrganismHealth().ToList();
            alteredEnvironmentCoordinates.AddRange(decreasedHealthCoordinates);

            // if organism has died, remove emitted sound
            foreach (var decreasedHealthCoordinate in decreasedHealthCoordinates)
            {
                if (!this.EcosystemData.HasLevel(decreasedHealthCoordinate, OrganismMeasure.Health))
                {
                    alteredEnvironmentCoordinates.AddRange(this.RemoveDistributedMeasure(decreasedHealthCoordinate, EnvironmentMeasure.Sound));
                }
            }

            return alteredEnvironmentCoordinates.Distinct();
        }

        private IEnumerable<Coordinate> OrganismsConsumeNutrients()
        {
            var organismCoordinates = this.EcosystemData.AliveOrganismCoordinates()
                .Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Nutrient)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var organismHealth = this.EcosystemData.GetLevel(organismCoordinate, OrganismMeasure.Health);
                var habitatNutrient = this.EcosystemData.GetLevel(organismCoordinate, EnvironmentMeasure.Nutrient);
                var desiredNutrient = 1 - organismHealth;
                var nutrientTaken = Math.Min(desiredNutrient, habitatNutrient); 

                var healthIncreased = this.EcosystemData.IncreaseLevel(organismCoordinate, OrganismMeasure.Health, nutrientTaken);
                var nutrientDecreased = this.EcosystemData.DecreaseLevel(organismCoordinate, EnvironmentMeasure.Nutrient, nutrientTaken);
                if (healthIncreased || nutrientDecreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> DecreasePheromoneLevel()
        {
            var pheromoneCoordinates = this.EcosystemData.AllCoordinates()
                .Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Pheromone)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var pheromoneCoordinate in pheromoneCoordinates)
            {
                var pheromoneDecreased = this.EcosystemData.DecreaseLevel(pheromoneCoordinate, EnvironmentMeasure.Pheromone, this.PheromoneFadeRate);
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

            var organismCoordinates = this.EcosystemData.DepositingPheromoneOrganismCoordinates().ToList();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var pheromoneIncreased = this.EcosystemData.IncreaseLevel(organismCoordinate, EnvironmentMeasure.Pheromone, this.PheromoneDepositRate);
                if (pheromoneIncreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseMineralLevels()
        {
            // only increase mineral where the terrain is not harmful (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var organismCoordinates = this.EcosystemData.OrganismCoordinates()
                .Where(coordinate => !this.EcosystemData.IsHarmful(coordinate)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var mineralIncreased = this.EcosystemData.IncreaseLevel(organismCoordinate, EnvironmentMeasure.Mineral, this.MineralFormRate);
                if (mineralIncreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> IncreaseNutrientLevels()
        {
            var nutrientCoordinates = this.EcosystemData.AllCoordinates()
                .Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Nutrient) 
                                     && !this.EcosystemData.IsHarmful(coordinate)).ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var nutrientCoordinate in nutrientCoordinates)
            {
                var nutrientIncreased = this.EcosystemData.IncreaseLevel(nutrientCoordinate, EnvironmentMeasure.Nutrient, this.NutrientGrowthRate);
                if (nutrientIncreased)
                {
                    alteredEnvironmentCoordinates.Add(nutrientCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> DecreaseOrganismHealth()
        {
            var organismCoordinates = this.EcosystemData.AliveOrganismCoordinates().ToList();

            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in organismCoordinates)
            {
                var healthDecreased = this.EcosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Health, this.HealthDeteriorationRate);
                if (healthDecreased)
                {
                    alteredEnvironmentCoordinates.Add(organismCoordinate);
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> ProgressWeatherAndHazards()
        {
            this.Weather.Progress();

            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach(var environmentMeasureHazardRate in this.EnvironmentMeasureHazardRates)
            {
                var environmentMeasure = environmentMeasureHazardRate.Key;
                var hazardRate = environmentMeasureHazardRate.Value;

                var weatherBiasedSpreadRate = hazardRate.SpreadRate;
                var weatherBiasedRemoveRate = hazardRate.RemoveRate;
                var weatherBiasedAddRate = hazardRate.AddRate;

                var weatherTrigger = environmentMeasure.WeatherTrigger;
                if (weatherTrigger != WeatherType.None)
                {
                    var weatherLevel = this.Weather.GetLevel(weatherTrigger);
                    weatherBiasedSpreadRate *= weatherLevel;
                    weatherBiasedRemoveRate *= (1 - weatherLevel);
                    weatherBiasedAddRate *= weatherLevel;
                }

                alteredEnvironmentCoordinates.AddRange(this.RandomSpreadHazards(environmentMeasure, weatherBiasedSpreadRate));
                alteredEnvironmentCoordinates.AddRange(this.RandomRemoveHazards(environmentMeasure, weatherBiasedRemoveRate));
                alteredEnvironmentCoordinates.AddRange(this.RandomAddHazards(environmentMeasure, weatherBiasedAddRate));
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> RandomSpreadHazards(EnvironmentMeasure environmentMeasure, double spreadChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!DecisionLogic.IsSuccessful(spreadChance))
                {
                    continue;
                }

                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(hazardCoordinate, 1, false, false).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(neighbourCoordinate =>
                    neighbourCoordinate != null
                    && !this.EcosystemData.HasLevel(neighbourCoordinate, EnvironmentMeasure.Obstruction)
                    && this.EcosystemData.GetLevel(neighbourCoordinate, environmentMeasure) < 1).ToList();
                if (validNeighbouringCoordinates.Count == 0)
                {
                    continue;
                }

                var chosenCoordinate = DecisionLogic.MakeDecision(validNeighbouringCoordinates);
                alteredEnvironmentCoordinates.AddRange(this.InsertDistributedMeasure(chosenCoordinate, environmentMeasure));
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> RandomRemoveHazards(EnvironmentMeasure environmentMeasure, double removeChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!DecisionLogic.IsSuccessful(removeChance))
                {
                    continue;
                }

                alteredEnvironmentCoordinates.AddRange(this.RemoveDistributedMeasure(hazardCoordinate, environmentMeasure));
            }

            // go through all remaining hazard coordinates and restore any remove measures that belonged to other hazards
            var remainingHazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            foreach (var remainingHazardCoordinate in remainingHazardCoordinates)
            {
                var radius = (this.EnvironmentMeasureDiameters[environmentMeasure] - 1) / 2;
                var neighbouringCoordinates = this.EcosystemData.GetNeighbours(remainingHazardCoordinate, radius, true, true).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(neighbouringCoordinate => neighbouringCoordinate != null).ToList();
                if (validNeighbouringCoordinates.Any(neighbouringCoordinate => 
                    this.EcosystemData.GetLevel(neighbouringCoordinate, environmentMeasure).Equals(0.0)))
                {
                    alteredEnvironmentCoordinates.AddRange(this.InsertDistributedMeasure(remainingHazardCoordinate, environmentMeasure));
                }
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> RandomAddHazards(EnvironmentMeasure environmentMeasure, double addChance)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            if (!DecisionLogic.IsSuccessful(addChance))
            {
                return alteredEnvironmentCoordinates;
            }

            var hazardCoordinates = this.EcosystemData.GetHazardCoordinates(environmentMeasure).ToList();
            var nonHazardCoordinates = this.EcosystemData.AllCoordinates().Except(hazardCoordinates);
            var chosenNonHazardCoordinate = DecisionLogic.MakeDecision(nonHazardCoordinates);
            if (!this.EcosystemData.HasLevel(chosenNonHazardCoordinate, EnvironmentMeasure.Obstruction))
            {
                alteredEnvironmentCoordinates.AddRange(this.InsertDistributedMeasure(chosenNonHazardCoordinate, environmentMeasure));
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> RemoveDistributedMeasure(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var diameter = this.EnvironmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;

            var neighbouringCoordinates = this.EcosystemData.GetNeighbours(coordinate, radius, true, true);
            for (var x = 0; x < diameter; x++)
            {
                for (var y = 0; y < diameter; y++)
                {
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];

                    if (neighbouringCoordinate == null)
                    {
                        continue;
                    }

                    this.EcosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, 0);
                    alteredEnvironmentCoordinates.Add(neighbouringCoordinate);
                }
            }

            if (environmentMeasure.IsHazardous)
            {
                this.EcosystemData.RemoveHazard(environmentMeasure, coordinate);
            }

            return alteredEnvironmentCoordinates;
        }

        public IEnumerable<Coordinate> InsertDistributedMeasure(Coordinate coordinate, EnvironmentMeasure environmentMeasure)
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var diameter = this.EnvironmentMeasureDiameters[environmentMeasure];
            var radius = (diameter - 1) / 2;

            var neighbouringCoordinates = this.EcosystemData.GetNeighbours(coordinate, radius, true, true);
            var gaussianKernel = new GaussianBlur(0.25 * diameter, diameter).Kernel;

            var gaussianCentre = (double)gaussianKernel[radius, radius];
            for (var x = 0; x < diameter; x++)
            {
                for (var y = 0; y < diameter; y++)
                {
                    var level = gaussianKernel[x, y] / gaussianCentre;
                    var neighbouringCoordinate = neighbouringCoordinates[x, y];

                    if (neighbouringCoordinate != null
                        && level > this.EcosystemData.GetLevel(neighbouringCoordinate, environmentMeasure))
                    {
                        this.EcosystemData.SetLevel(neighbouringCoordinate, environmentMeasure, level);
                        alteredEnvironmentCoordinates.Add(neighbouringCoordinate);
                    }
                }
            }

            if (environmentMeasure.IsHazardous)
            {
                this.EcosystemData.InsertHazard(environmentMeasure, coordinate);
            }

            return alteredEnvironmentCoordinates;
        }

        public void SetLevel(Coordinate coordinate, EnvironmentMeasure environmentMeasure, double level)
        {
            this.EcosystemData.SetLevel(coordinate, environmentMeasure, level);
        }

        public void SetMeasureBias(OrganismMeasure organismMeasure, double bias)
        {
            this.MeasureBiases[organismMeasure] = bias;
        }

        public HazardRate GetHazardRate(EnvironmentMeasure environmentMeasure)
        {
            if (this.EnvironmentMeasureHazardRates.ContainsKey(environmentMeasure))
            {
                return this.EnvironmentMeasureHazardRates[environmentMeasure];
            }

            throw new ArgumentException(string.Format("No hazard rate for environment measure {0}", environmentMeasure), "environmentMeasure");
        }

        public void SetHazardRate(EnvironmentMeasure environmentMeasure, HazardRate hazardChance)
        {
            if (this.EnvironmentMeasureHazardRates.ContainsKey(environmentMeasure))
            {
                this.EnvironmentMeasureHazardRates[environmentMeasure] = hazardChance;
            }
            else
            {
                throw new ArgumentException(string.Format("No hazard rate for environment measure {0}", environmentMeasure), "environmentMeasure");
            }
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.EcosystemData.OrganismCoordinates().Count());
        }
    }
}