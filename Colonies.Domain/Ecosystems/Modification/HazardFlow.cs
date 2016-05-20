namespace Wacton.Colonies.Domain.Ecosystems.Modification
{
    using System.Linq;

    using Wacton.Colonies.Domain.Extensions;
    using Wacton.Colonies.Domain.Measures;
    using Wacton.Colonies.Domain.Settings;
    using Wacton.Colonies.Domain.Weathers;
    using Wacton.Tovarisch.Randomness;

    public class HazardFlow
    {
        private readonly EcosystemData ecosystemData;
        private readonly EcosystemSettings ecosystemSettings;
        private readonly Distributor distributor;
        private readonly IWeather weather;

        public HazardFlow(EcosystemData ecosystemData, EcosystemSettings ecosystemSettings, Distributor distributor, IWeather weather)
        {
            this.ecosystemData = ecosystemData;
            this.ecosystemSettings = ecosystemSettings;
            this.distributor = distributor;
            this.weather = weather;
        }

        public void Advance()
        {
            foreach (var environmentMeasureHazardRate in this.ecosystemSettings.HazardRates)
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

                this.RandomlySpreadHazards(environmentMeasure, weatherBiasedSpreadRate);
                this.RandomlyRemoveHazards(environmentMeasure, weatherBiasedRemoveRate);
                this.RandomlyInsertHazards(environmentMeasure, weatherBiasedAddRate);
            }
        }

        private void RandomlyInsertHazards(EnvironmentMeasure environmentMeasure, double addChance)
        {
            if (!RandomSelection.IsSuccessful(addChance))
            {
                return;
            }

            var hazardCoordinates = this.distributor.HazardSources(environmentMeasure).ToList();
            var nonHazardCoordinates = this.ecosystemData.AllCoordinates().Except(hazardCoordinates);
            var chosenNonHazardCoordinate = RandomSelection.SelectOne(nonHazardCoordinates);
            if (!this.ecosystemData.HasLevel(chosenNonHazardCoordinate, EnvironmentMeasure.Obstruction))
            {
                this.distributor.Insert(environmentMeasure, chosenNonHazardCoordinate);
            }
        }

        private void RandomlySpreadHazards(EnvironmentMeasure environmentMeasure, double spreadChance)
        {
            var hazardCoordinates = this.distributor.HazardSources(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!RandomSelection.IsSuccessful(spreadChance))
                {
                    continue;
                }

                var neighbouringCoordinates = this.ecosystemData.GetNeighbours(hazardCoordinate, 1, false, false).ToList();
                var validNeighbouringCoordinates = neighbouringCoordinates.Where(neighbourCoordinate =>
                    neighbourCoordinate != null
                    && !this.ecosystemData.HasLevel(neighbourCoordinate, EnvironmentMeasure.Obstruction)
                    && this.ecosystemData.GetLevel(neighbourCoordinate, environmentMeasure) < 1).ToList();

                if (validNeighbouringCoordinates.Count == 0)
                {
                    continue;
                }

                var chosenCoordinate = RandomSelection.SelectOne(validNeighbouringCoordinates);
                this.distributor.Insert(environmentMeasure, chosenCoordinate);
            }
        }

        private void RandomlyRemoveHazards(EnvironmentMeasure environmentMeasure, double removeChance)
        {
            var hazardCoordinates = this.distributor.HazardSources(environmentMeasure).ToList();
            foreach (var hazardCoordinate in hazardCoordinates)
            {
                if (!RandomSelection.IsSuccessful(removeChance))
                {
                    continue;
                }

                this.distributor.Remove(environmentMeasure, hazardCoordinate);
            }
        }
    }
}