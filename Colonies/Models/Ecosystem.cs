namespace Wacton.Colonies.Models
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
        private IEcosystemHistoryPuller EcosystemHistoryPuller { get; set; }
        public IWeather Weather { get; private set; }
        public EnvironmentMeasureDistributor EnvironmentMeasureDistributor { get; private set; }
        private OrganismEnvironmentProcessor OrganismEnvironmentProcessor { get; set; }

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

        private readonly List<Action> updateFunctions;
        public int UpdateStages { get { return updateFunctions.Count; } }
        private int updateCount = 0;

        public Ecosystem(EcosystemData ecosystemData, IEcosystemHistoryPuller ecosystemHistory, IWeather weather, EnvironmentMeasureDistributor environmentMeasureDistributor, OrganismEnvironmentProcessor organismEnvironmentProcessor)
        {
            this.EcosystemData = ecosystemData;
            this.EcosystemHistoryPuller = ecosystemHistory;
            this.Weather = weather;
            this.EnvironmentMeasureDistributor = environmentMeasureDistributor;
            this.OrganismEnvironmentProcessor = organismEnvironmentProcessor;

            this.MeasureBiases = new Dictionary<OrganismMeasure, double> { { OrganismMeasure.Health, 1 }, { OrganismMeasure.Inventory, 0} };

            this.HealthDeteriorationRate = 1 / 750.0;
            this.PheromoneDepositRate = 1 / 10.0;
            this.PheromoneFadeRate = 1 / 300.0;
            this.NutrientGrowthRate = 1 / 500.0;
            this.MineralFormRate = 1 / 100.0;
            this.ObstructionDemolishRate = 1 / 5.0;

            this.EnvironmentMeasureHazardRates = new Dictionary<EnvironmentMeasure, HazardRate>
                {
                    { EnvironmentMeasure.Damp, new HazardRate(1 / 2000.0, 1 / 500.0, 1 / 1000.0) },
                    { EnvironmentMeasure.Heat, new HazardRate(1 / 2000.0, 1 / 500.0, 1 / 1000.0) },
                    { EnvironmentMeasure.Poison, new HazardRate(0.0, 1 / 50.0, 1 / 50.0) }
                };

            this.updateFunctions = new List<Action>
                                       {
                                           this.PerformEnvironmentInteractions,
                                           this.PerformMovementsActions,
                                           this.PerformOrganismInteractions,
                                           this.PerformEcosystemModifiers
                                       };
        }

        public UpdateSummary UpdateOneStage()
        {
            var updateIndex = this.updateCount % this.UpdateStages;
            var updateFunction = this.updateFunctions[updateIndex];
            updateFunction.Invoke();
            var ecosystemHistory = this.EcosystemHistoryPuller.Pull();
            var updateSummary = new UpdateSummary(this.updateCount, ecosystemHistory, this.EcosystemData.OrganismCoordinatePairs());
            this.updateCount++;
            return updateSummary;
        }

        private void RefreshOrganismIntentions()
        {
            var aliveOrganismCoordinates = this.EcosystemData.AliveOrganismCoordinates();
            foreach (var aliveOrganismCoordinate in aliveOrganismCoordinates)
            {
                var organism = this.EcosystemData.GetOrganism(aliveOrganismCoordinate);
                var environment = this.EcosystemData.GetEnvironment(aliveOrganismCoordinate);
                organism.RefreshIntention(environment);
            }
        }

        private void PerformEnvironmentInteractions()
        {
            // remove sound distribution before refreshing intentions, insert them again afterwards if still need assistance
            var aliveOrganismCoordinates = this.EcosystemData.AliveOrganismCoordinates().ToList();
            foreach (var aliveOrganismCoordinate in aliveOrganismCoordinates)
            {
                var organism = this.EcosystemData.GetOrganism(aliveOrganismCoordinate);
                if (organism.NeedsAssistance)
                {
                    this.EnvironmentMeasureDistributor.RemoveDistribution(aliveOrganismCoordinate, EnvironmentMeasure.Sound);
                }
            }

            this.RefreshOrganismIntentions();

            foreach (var aliveOrganismCoordinate in aliveOrganismCoordinates)
            {
                var organism = this.EcosystemData.GetOrganism(aliveOrganismCoordinate);
                this.OrganismEnvironmentProcessor.Process(aliveOrganismCoordinate);

                if (organism.NeedsAssistance)
                {
                    this.EnvironmentMeasureDistributor.InsertDistribution(aliveOrganismCoordinate, EnvironmentMeasure.Sound);
                }
            }
        }

        private void PerformMovementsActions()
        {
            var desiredOrganismCoordinates = EcosystemLogic.GetDesiredCoordinates(this.EcosystemData);
            var movedOrganismCoordinates = EcosystemLogic.ResolveOrganismHabitats(this.EcosystemData, desiredOrganismCoordinates, new List<IOrganism>(), this);

            this.IncreasePheromoneLevels();
            this.IncreaseMineralLevels();

            foreach (var movedOrganismCoordinate in movedOrganismCoordinates)
            {
                this.EcosystemData.MoveOrganism(movedOrganismCoordinate.Key, movedOrganismCoordinate.Value);
            }

            // for any organisms that attempted to move to an obstructed habitat, decrease obstruction level
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction));
            foreach (var obstructedCoordinate in obstructedCoordinates)
            {
                this.EcosystemData.DecreaseLevel(obstructedCoordinate, EnvironmentMeasure.Obstruction, this.ObstructionDemolishRate);
            }

            var soundSourceCoordinates = this.EcosystemData.EmittingSoundOrganismCoordinates();
            foreach (var soundSourceCoordinate in soundSourceCoordinates)
            {
                this.EnvironmentMeasureDistributor.InsertDistribution(soundSourceCoordinate, EnvironmentMeasure.Sound);
            }
        }

        private void PerformOrganismInteractions()
        {
            this.RefreshOrganismIntentions();
            this.NourishNeighbours();
        }

        private void PerformEcosystemModifiers()
        {
            this.DecreasePheromoneLevel();
            this.IncreaseNutrientLevels();
            this.ProgressWeatherAndHazards();

            var emittingSoundOrganismCoordinates = this.EcosystemData.EmittingSoundOrganismCoordinates();
            this.DecreaseOrganismHealth();
            var deadOrganismCoordinates = this.EcosystemData.DeadOrganismCoordinates();

            // if organism has died, remove emitted sound
            foreach (var recentlyDiedOrganismCoordinate in emittingSoundOrganismCoordinates.Intersect(deadOrganismCoordinates))
            {
                this.EnvironmentMeasureDistributor.RemoveDistribution(recentlyDiedOrganismCoordinate, EnvironmentMeasure.Sound);
            }
        }

        private void NourishNeighbours()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            foreach (var organismCoordinate in this.EcosystemData.OrganismCoordinates(Intention.Nourish))
            {
                var neighboursRequestingNutrient = this.GetNeighboursRequestingNutrient(organismCoordinate);
                if (neighboursRequestingNutrient.Any())
                {
                    var nourishedOrganism = neighboursRequestingNutrient.FirstOrDefault() ?? DecisionLogic.MakeDecision(neighboursRequestingNutrient);
                    var nourishedOrganismCoordinate = this.EcosystemData.CoordinateOf(nourishedOrganism);
                        
                    var desiredNutrient = 1 - this.EcosystemData.GetLevel(nourishedOrganismCoordinate, OrganismMeasure.Health);
                    var availableNutrient = this.EcosystemData.GetLevel(organismCoordinate, OrganismMeasure.Inventory);
                    var givenNutrient = Math.Min(desiredNutrient, availableNutrient);
                    this.EcosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Inventory, givenNutrient);
                    this.EcosystemData.IncreaseLevel(nourishedOrganismCoordinate, OrganismMeasure.Health, givenNutrient);
                        
                    alteredEnvironmentCoordinates.Add(nourishedOrganismCoordinate);
                    if (!nourishedOrganism.NeedsAssistance)
                    {
                        this.EnvironmentMeasureDistributor.RemoveDistribution(nourishedOrganismCoordinate, EnvironmentMeasure.Sound);
                    }
                }

                alteredEnvironmentCoordinates.Add(organismCoordinate);
            }
        }

        private List<IOrganism> GetNeighboursRequestingNutrient(Coordinate coordinate)
        {
            // TODO: better way?!
            var neighbourCoordinates = this.EcosystemData.GetValidNeighbours(coordinate, 1, false, false).ToList();
            var neighbourOrganisms = neighbourCoordinates.Select(this.EcosystemData.GetOrganism).Where(organism => organism != null).ToList();
            return neighbourOrganisms.Where(neighbour => neighbour.Intention.Equals(Intention.Reproduce) && neighbour.NeedsAssistance).ToList();
        }

        private void DecreasePheromoneLevel()
        {
            var pheromoneCoordinates = this.EcosystemData.AllCoordinates()
                .Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Pheromone)).ToList();

            foreach (var pheromoneCoordinate in pheromoneCoordinates)
            {
                this.EcosystemData.DecreaseLevel(pheromoneCoordinate, EnvironmentMeasure.Pheromone, this.PheromoneFadeRate);
            }
        }

        private void IncreasePheromoneLevels()
        {
            var organismCoordinates = this.EcosystemData.DepositingPheromoneOrganismCoordinates().ToList();
            foreach (var organismCoordinate in organismCoordinates)
            {
                this.EcosystemData.IncreaseLevel(organismCoordinate, EnvironmentMeasure.Pheromone, this.PheromoneDepositRate);
            }
        }

        private void IncreaseMineralLevels()
        {
            // only increase mineral where the terrain is not harmful (even when the organism is dead!)
            // TODO: need a "HasDecomposed" bool - this could stop showing organism and stop mineral form
            var organismCoordinates = this.EcosystemData.OrganismCoordinates()
                .Where(coordinate => !this.EcosystemData.IsHarmful(coordinate)).ToList();

            foreach (var organismCoordinate in organismCoordinates)
            {
                this.EcosystemData.IncreaseLevel(organismCoordinate, EnvironmentMeasure.Mineral, this.MineralFormRate);
            }
        }

        private void IncreaseNutrientLevels()
        {
            var nutrientCoordinates = this.EcosystemData.AllCoordinates()
                .Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Nutrient) 
                                     && !this.EcosystemData.IsHarmful(coordinate)).ToList();

            foreach (var nutrientCoordinate in nutrientCoordinates)
            {
                this.EcosystemData.IncreaseLevel(nutrientCoordinate, EnvironmentMeasure.Nutrient, this.NutrientGrowthRate);
            }
        }

        private void DecreaseOrganismHealth()
        {
            var organismCoordinates = this.EcosystemData.AliveOrganismCoordinates().ToList();
            foreach (var organismCoordinate in organismCoordinates)
            {
                this.EcosystemData.DecreaseLevel(organismCoordinate, OrganismMeasure.Health, this.HealthDeteriorationRate);
            }
        }

        private void ProgressWeatherAndHazards()
        {
            this.Weather.Progress();

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

                this.EnvironmentMeasureDistributor.RandomSpreadHazards(environmentMeasure, weatherBiasedSpreadRate);
                this.EnvironmentMeasureDistributor.RandomRemoveHazards(environmentMeasure, weatherBiasedRemoveRate);
                this.EnvironmentMeasureDistributor.RandomAddHazards(environmentMeasure, weatherBiasedAddRate);
            }
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