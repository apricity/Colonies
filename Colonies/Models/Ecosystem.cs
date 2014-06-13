﻿namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Logic;
    using Wacton.Colonies.Models.DataProviders;
    using Wacton.Colonies.Models.Interfaces;

    public class Ecosystem : IEcosystem
    {
        private EcosystemData EcosystemData { get; set; }
        public IWeather Weather { get; private set; }
        public EnvironmentMeasureDistributor EnvironmentMeasureDistributor { get; private set; }

        public Dictionary<OrganismMeasure, double> MeasureBiases { get; private set; }

        // TODO: neater management of these?
        public double HealthDeteriorationRate { get; set; }
        public double PheromoneDepositRate { get; set; }
        public double PheromoneFadeRate { get; set; }
        public double NutrientGrowthRate { get; set; }
        public double MineralFormRate { get; set; }
        public double ObstructionDemolishRate { get; set; }

        public Dictionary<EnvironmentMeasure, HazardRate> EnvironmentMeasureHazardRates { get; private set; }

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

        public Ecosystem(EcosystemData ecosystemData, IWeather weather, EnvironmentMeasureDistributor environmentMeasureDistributor)
        {
            this.EcosystemData = ecosystemData;
            this.Weather = weather;
            this.EnvironmentMeasureDistributor = environmentMeasureDistributor;

            this.MeasureBiases = new Dictionary<OrganismMeasure, double> { { OrganismMeasure.Health, 1 } };

            this.HealthDeteriorationRate = 1 / 500.0;
            this.PheromoneDepositRate = 1 / 10.0;
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

        private IEnumerable<Coordinate> PerformOrganismIntentionActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();
            foreach (var organismCoordinate in this.EcosystemData.AliveOrganismCoordinates())
            {
                var organism = this.EcosystemData.GetOrganism(organismCoordinate);
                var environment = this.EcosystemData.GetEnvironment(organismCoordinate);
                var reducedMeasures = organism.PerformIntentionAction(environment);
                foreach (var reducedMeasure in reducedMeasures)
                {
                    this.EcosystemData.DecreaseLevel(organismCoordinate, reducedMeasure.Key, reducedMeasure.Value);
                }

                alteredEnvironmentCoordinates.Add(organismCoordinate);
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformPreMovementActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();
            alteredEnvironmentCoordinates.AddRange(this.PerformOrganismIntentionActions());
            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformMovementsActions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var existingSoundSourceCoordinate = this.EcosystemData.EmittingSoundOrganismCoordinates().ToList();

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

            foreach (var sourceCoordinate in existingSoundSourceCoordinate)
            {
                alteredEnvironmentCoordinates.AddRange(this.EnvironmentMeasureDistributor.RemoveDistribution(sourceCoordinate, EnvironmentMeasure.Sound));
            }

            var updatedSoundSourceCoordinates = this.EcosystemData.EmittingSoundOrganismCoordinates().ToList();
            foreach (var sourceCoordinate in updatedSoundSourceCoordinates)
            {
                alteredEnvironmentCoordinates.AddRange(this.EnvironmentMeasureDistributor.InsertDistribution(sourceCoordinate, EnvironmentMeasure.Sound));
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
                    alteredEnvironmentCoordinates.AddRange(this.EnvironmentMeasureDistributor.RemoveDistribution(decreasedHealthCoordinate, EnvironmentMeasure.Sound));
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
                var organism = this.EcosystemData.GetOrganism(organismCoordinate);
                var habitatNutrient = this.EcosystemData.GetLevel(organismCoordinate, EnvironmentMeasure.Nutrient);
                var nutrientTaken = organism.ProcessNutrient(habitatNutrient);

                if (nutrientTaken.Equals(0.0))
                {
                    continue;
                }

                var nutrientDecreased = this.EcosystemData.DecreaseLevel(organismCoordinate, EnvironmentMeasure.Nutrient, nutrientTaken);
                if (nutrientDecreased)
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

                alteredEnvironmentCoordinates.AddRange(this.EnvironmentMeasureDistributor.RandomSpreadHazards(environmentMeasure, weatherBiasedSpreadRate));
                alteredEnvironmentCoordinates.AddRange(this.EnvironmentMeasureDistributor.RandomRemoveHazards(environmentMeasure, weatherBiasedRemoveRate));
                alteredEnvironmentCoordinates.AddRange(this.EnvironmentMeasureDistributor.RandomAddHazards(environmentMeasure, weatherBiasedAddRate));
            }

            return alteredEnvironmentCoordinates;
        }

        public void SetLevel(Coordinate coordinate, EnvironmentMeasure environmentMeasure, double level)
        {
            this.EcosystemData.SetLevel(coordinate, environmentMeasure, level);
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