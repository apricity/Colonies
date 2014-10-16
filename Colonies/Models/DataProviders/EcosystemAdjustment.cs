namespace Wacton.Colonies.Models.DataProviders
{
    using System.Linq;

    using Wacton.Colonies.DataTypes;
    using Wacton.Colonies.DataTypes.Enums;
    using Wacton.Colonies.Models.Interfaces;

    public class EcosystemAdjustment : IEcosystemStage
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemRates ecosystemRates;
        private readonly EnvironmentMeasureDistributor environmentMeasureDistributor;
        private readonly IWeather weather;

        public EcosystemAdjustment(EcosystemData ecosystemData, EcosystemRates ecosystemRates, EnvironmentMeasureDistributor environmentMeasureDistributor, IWeather weather)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemRates = ecosystemRates;
            this.environmentMeasureDistributor = environmentMeasureDistributor;
            this.weather = weather;
        }

        public void Execute()
        {
            this.DecreasePheromoneLevel();
            this.IncreaseNutrientLevels();
            this.ProgressWeatherAndHazards();

            var needingAssistanceOrganismCoordinates = this.ecosystemData.NeedingAssistanceOrganismCoordinates();
            this.DecreaseOrganismHealth();
            var deadOrganismCoordinates = this.ecosystemData.DeadOrganismCoordinates();

            // if organism has died, remove sound
            foreach (var recentlyDiedOrganismCoordinate in needingAssistanceOrganismCoordinates.Intersect(deadOrganismCoordinates))
            {
                this.environmentMeasureDistributor.RemoveDistribution(recentlyDiedOrganismCoordinate, EnvironmentMeasure.Sound);
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
            var organismCoordinates = this.ecosystemData.AliveOrganismCoordinates().ToList();
            this.ecosystemData.AdjustLevels(organismCoordinates, OrganismMeasure.Health, -this.ecosystemRates.DecreasingRates[OrganismMeasure.Health]);
        }

        private void ProgressWeatherAndHazards()
        {
            this.weather.Progress();

            foreach (var environmentMeasureHazardRate in this.ecosystemRates.HazardRates)
            {
                var environmentMeasure = environmentMeasureHazardRate.Key;
                var hazardRate = environmentMeasureHazardRate.Value;

                var weatherBiasedSpreadRate = hazardRate.SpreadRate;
                var weatherBiasedRemoveRate = hazardRate.RemoveRate;
                var weatherBiasedAddRate = hazardRate.AddRate;

                var weatherTrigger = environmentMeasure.WeatherTrigger;
                if (weatherTrigger != WeatherType.None)
                {
                    var weatherLevel = this.weather.GetLevel(weatherTrigger);
                    weatherBiasedSpreadRate *= weatherLevel;
                    weatherBiasedRemoveRate *= (1 - weatherLevel);
                    weatherBiasedAddRate *= weatherLevel;
                }

                this.environmentMeasureDistributor.RandomSpreadHazards(environmentMeasure, weatherBiasedSpreadRate);
                this.environmentMeasureDistributor.RandomRemoveHazards(environmentMeasure, weatherBiasedRemoveRate);
                this.environmentMeasureDistributor.RandomAddHazards(environmentMeasure, weatherBiasedAddRate);
            }
        }
    }
}