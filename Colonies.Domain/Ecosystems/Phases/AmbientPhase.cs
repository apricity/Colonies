namespace Wacton.Colonies.Domain.Ecosystems.Phases
{
    using System.Linq;

    using Wacton.Colonies.Domain.Ecosystems.Modification;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Settings;
    using Wacton.Colonies.Domain.Weathers;

    public class AmbientPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemSettings ecosystemSettings;
        private readonly Distributor distributor;
        private readonly IWeather weather;
        private readonly HazardFlow hazardFlow;

        public AmbientPhase(EcosystemData ecosystemData, EcosystemSettings ecosystemSettings, Distributor distributor, IWeather weather, HazardFlow hazardFlow)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemSettings = ecosystemSettings;
            this.distributor = distributor;
            this.weather = weather;
            this.hazardFlow = hazardFlow;
        }

        public void Execute()
        {
            this.DecreasePheromoneLevel();
            this.IncreaseNutrientLevels();
            this.ProgressWeatherAndHazards();

            var audibleOrganismCoordinates = this.ecosystemData.AudibleOrganismCoordinates();
            this.DecreaseOrganismHealth();
            var deadOrganismCoordinates = this.ecosystemData.DeadOrganismCoordinates();

            // if organism has died, remove sound
            foreach (var recentlyDiedOrganismCoordinate in audibleOrganismCoordinates.Intersect(deadOrganismCoordinates))
            {
                this.distributor.Remove(EnvironmentMeasure.Sound, recentlyDiedOrganismCoordinate);
            }
        }

        private void DecreasePheromoneLevel()
        {
            var pheromoneCoordinates = this.ecosystemData.AllCoordinates()
                .Where(coordinate => this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Pheromone)).ToList();
            this.ecosystemData.AdjustLevels(pheromoneCoordinates, EnvironmentMeasure.Pheromone, -this.ecosystemSettings.DecreasingRates[EnvironmentMeasure.Pheromone]);
        }

        private void IncreaseNutrientLevels()
        {
            var nutrientCoordinates = this.ecosystemData.AllCoordinates()
                .Where(coordinate => this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Nutrient)
                                     && !this.ecosystemData.IsHarmful(coordinate)).ToList();
            this.ecosystemData.AdjustLevels(nutrientCoordinates, EnvironmentMeasure.Nutrient, this.ecosystemSettings.IncreasingRates[EnvironmentMeasure.Nutrient]);
        }

        private void DecreaseOrganismHealth()
        {
            var aliveOrganismCoordinates = this.ecosystemData.AliveOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(aliveOrganismCoordinates, OrganismMeasure.Health, -this.ecosystemSettings.DecreasingRates[OrganismMeasure.Health]);

            // decrease organism health even more if diseased (currently triple health loss)
            var diseasedOrganismCoordinates = this.ecosystemData.DiseasedOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(diseasedOrganismCoordinates, OrganismMeasure.Health, -this.ecosystemSettings.DecreasingRates[OrganismMeasure.Health] * 2);
        }

        private void ProgressWeatherAndHazards()
        {
            this.weather.Advance();
            this.hazardFlow.Advance();
        }
    }
}