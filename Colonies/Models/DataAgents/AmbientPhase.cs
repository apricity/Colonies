namespace Wacton.Colonies.Models.DataAgents
{
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class AmbientPhase : IEcosystemPhase
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemRates ecosystemRates;
        private readonly Distributor distributor;
        private readonly IWeather weather;
        private readonly HazardFlow hazardFlow;

        public AmbientPhase(EcosystemData ecosystemData, EcosystemRates ecosystemRates, Distributor distributor, IWeather weather, HazardFlow hazardFlow)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemRates = ecosystemRates;
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
            this.ecosystemData.AdjustLevels(pheromoneCoordinates, EnvironmentMeasure.Pheromone, -this.ecosystemRates.DecreasingRates[EnvironmentMeasure.Pheromone]);
        }

        private void IncreaseNutrientLevels()
        {
            var nutrientCoordinates = this.ecosystemData.AllCoordinates()
                .Where(coordinate => this.ecosystemData.HasLevel(coordinate, EnvironmentMeasure.Nutrient)
                                     && !this.ecosystemData.IsHarmful(coordinate)).ToList();
            this.ecosystemData.AdjustLevels(nutrientCoordinates, EnvironmentMeasure.Nutrient, this.ecosystemRates.IncreasingRates[EnvironmentMeasure.Nutrient]);
        }

        private void DecreaseOrganismHealth()
        {
            var aliveOrganismCoordinates = this.ecosystemData.AliveOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(aliveOrganismCoordinates, OrganismMeasure.Health, -this.ecosystemRates.DecreasingRates[OrganismMeasure.Health]);

            // decrease organism health even more if diseased (currently triple health loss)
            var diseasedOrganismCoordinates = this.ecosystemData.DiseasedOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(diseasedOrganismCoordinates, OrganismMeasure.Health, -this.ecosystemRates.DecreasingRates[OrganismMeasure.Health] * 2);
        }

        private void ProgressWeatherAndHazards()
        {
            this.weather.Advance();
            this.hazardFlow.Advance();
        }
    }
}