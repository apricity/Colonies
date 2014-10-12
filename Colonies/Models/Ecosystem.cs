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

        private readonly List<Func<IEnumerable<Coordinate>>> updateFunctions;
        public int UpdateStages { get { return updateFunctions.Count; } }
        private int updateCount = 0;

        public Ecosystem(EcosystemData ecosystemData, IWeather weather, EnvironmentMeasureDistributor environmentMeasureDistributor, OrganismEnvironmentProcessor organismEnvironmentProcessor)
        {
            this.EcosystemData = ecosystemData;
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

            this.updateFunctions = new List<Func<IEnumerable<Coordinate>>>
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
            var previousOrganismCoordinates = this.EcosystemData.OrganismCoordinatePairs();
            var alteredEnvironmentCoordinates = updateFunction.Invoke().ToList();
            var currentOrganismCoordinates = this.EcosystemData.OrganismCoordinatePairs();
            var updateSummary = new UpdateSummary(this.updateCount, previousOrganismCoordinates, currentOrganismCoordinates, alteredEnvironmentCoordinates);
            this.updateCount++;
            return updateSummary;
        }

        private IEnumerable<Coordinate> RefreshOrganismIntentions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            var aliveOrganismCoordinates = this.EcosystemData.AliveOrganismCoordinates();
            foreach (var aliveOrganismCoordinate in aliveOrganismCoordinates)
            {
                var organism = this.EcosystemData.GetOrganism(aliveOrganismCoordinate);
                var environment = this.EcosystemData.GetEnvironment(aliveOrganismCoordinate);
                organism.RefreshIntention(environment);
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformEnvironmentInteractions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();

            // remove sound distribution before refreshing intentions, insert them again afterwards if still need assistance
            var aliveOrganismCoordinates = this.EcosystemData.AliveOrganismCoordinates();
            foreach (var aliveOrganismCoordinate in aliveOrganismCoordinates)
            {
                var organism = this.EcosystemData.GetOrganism(aliveOrganismCoordinate);
                if (organism.NeedsAssistance)
                {
                    var ecosystemModification = this.EnvironmentMeasureDistributor.RemoveDistribution(aliveOrganismCoordinate, EnvironmentMeasure.Sound);
                    var modifiedCoordinates = this.Modify(ecosystemModification);
                    alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);
                }

                alteredEnvironmentCoordinates.Add(aliveOrganismCoordinate);
            }

            alteredEnvironmentCoordinates.AddRange(this.RefreshOrganismIntentions());

            foreach (var aliveOrganismCoordinate in aliveOrganismCoordinates)
            {
                var organism = this.EcosystemData.GetOrganism(aliveOrganismCoordinate);
                var ecosystemModification = this.OrganismEnvironmentProcessor.Process(aliveOrganismCoordinate);
                var modifiedCoordinates = this.Modify(ecosystemModification);
                alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);

                if (organism.NeedsAssistance)
                {
                    ecosystemModification = this.EnvironmentMeasureDistributor.InsertDistribution(aliveOrganismCoordinate, EnvironmentMeasure.Sound);
                    modifiedCoordinates = this.Modify(ecosystemModification);
                    alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);
                }

                alteredEnvironmentCoordinates.Add(aliveOrganismCoordinate);
            }

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
            var obstructedCoordinates = desiredOrganismCoordinates.Values.Where(coordinate => this.EcosystemData.HasLevel(coordinate, EnvironmentMeasure.Obstruction));
            foreach (var obstructedCoordinate in obstructedCoordinates)
            {
                var obstructionDecreased = this.EcosystemData.DecreaseLevel(obstructedCoordinate, EnvironmentMeasure.Obstruction, this.ObstructionDemolishRate);
                if (obstructionDecreased)
                {
                    alteredEnvironmentCoordinates.Add(obstructedCoordinate);
                }
            }

            var soundSourceCoordinates = this.EcosystemData.EmittingSoundOrganismCoordinates();
            foreach (var soundSourceCoordinate in soundSourceCoordinates)
            {
                var ecosystemModification = this.EnvironmentMeasureDistributor.InsertDistribution(soundSourceCoordinate, EnvironmentMeasure.Sound);
                var modifiedCoordinates = this.Modify(ecosystemModification);
                alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);
            }

            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformOrganismInteractions()
        {
            var alteredEnvironmentCoordinates = new List<Coordinate>();
            alteredEnvironmentCoordinates.AddRange(this.RefreshOrganismIntentions());
            alteredEnvironmentCoordinates.AddRange(this.NourishNeighbours());
            return alteredEnvironmentCoordinates;
        }

        private IEnumerable<Coordinate> PerformEcosystemModifiers()
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
                    var ecosystemModification = this.EnvironmentMeasureDistributor.RemoveDistribution(decreasedHealthCoordinate, EnvironmentMeasure.Sound);
                    var modifiedCoordinates = this.Modify(ecosystemModification);
                    alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);
                }
            }

            return alteredEnvironmentCoordinates.Distinct();
        }

        private IEnumerable<Coordinate> NourishNeighbours()
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
                        var ecosystemModification = this.EnvironmentMeasureDistributor.RemoveDistribution(nourishedOrganismCoordinate, EnvironmentMeasure.Sound);
                        var modifiedCoordinates = this.Modify(ecosystemModification);
                        alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);
                    }
                }

                alteredEnvironmentCoordinates.Add(organismCoordinate);
            }

            return alteredEnvironmentCoordinates;
        }

        private List<IOrganism> GetNeighboursRequestingNutrient(Coordinate coordinate)
        {
            // TODO: better way?!
            var neighbourCoordinates = this.EcosystemData.GetValidNeighbours(coordinate, 1, false, false).ToList();
            var neighbourOrganisms = neighbourCoordinates.Select(this.EcosystemData.GetOrganism).Where(organism => organism != null).ToList();
            return neighbourOrganisms.Where(neighbour => neighbour.Intention.Equals(Intention.Reproduce) && neighbour.NeedsAssistance).ToList();
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

                // TODO: repair broken hazards should not be needed - must be able to find a way while calculating removing hazards...
                var modifiedCoordinates = new List<Coordinate>();
                modifiedCoordinates.AddRange(this.Modify(this.EnvironmentMeasureDistributor.RandomSpreadHazards(environmentMeasure, weatherBiasedSpreadRate)));
                modifiedCoordinates.AddRange(this.Modify(this.EnvironmentMeasureDistributor.RandomRemoveHazards(environmentMeasure, weatherBiasedRemoveRate)));
                modifiedCoordinates.AddRange(this.Modify(this.EnvironmentMeasureDistributor.RepairBrokenHazards(environmentMeasure)));
                modifiedCoordinates.AddRange(this.Modify(this.EnvironmentMeasureDistributor.RandomAddHazards(environmentMeasure, weatherBiasedAddRate)));
                alteredEnvironmentCoordinates.AddRange(modifiedCoordinates);
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

        public IEnumerable<Coordinate> Modify(EcosystemModification ecosystemModification)
        {
            this.EcosystemData.Modify(ecosystemModification);

            var modifiedCoordinates = new List<Coordinate>();
            modifiedCoordinates.AddRange(ecosystemModification.EnvironmentModifications.Select(mod => mod.Coordinate));
            modifiedCoordinates.AddRange(ecosystemModification.OrganismModifications.Select(mod => mod.Coordinate));
            return modifiedCoordinates;
        }

        public override String ToString()
        {
            return string.Format("{0}x{1} : {2} organisms", this.Width, this.Height, this.EcosystemData.OrganismCoordinates().Count());
        }
    }
}