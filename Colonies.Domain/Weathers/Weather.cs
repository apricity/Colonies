namespace Wacton.Colonies.Domain.Weathers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Weather : IWeather
    {
        // TODO: better naming?
        private readonly Dictionary<WeatherType, double> weatherChangeRates;
        private readonly Dictionary<WeatherType, double> weatherLevels;

        public Weather()
        {
            this.weatherChangeRates = new Dictionary<WeatherType, double>
                                          {
                                              { WeatherType.Damp, 1 / (double)200 },
                                              { WeatherType.Heat, 1 / (double)2000 }
                                          };

            this.weatherLevels = new Dictionary<WeatherType, double>();
            foreach (var weatherType in this.weatherChangeRates.Keys.ToList())
            {
                this.weatherLevels.Add(weatherType, 0);
            }
        }

        public double GetLevel(WeatherType weatherType)
        {
            return this.weatherLevels[weatherType];
        }

        public void Advance()
        {
            var ratesToInvert = this.weatherChangeRates.ToDictionary(
                weatherChangeRate => weatherChangeRate.Key, weatherChangeRate => false);

            foreach (var weatherChangeRate in this.weatherChangeRates)
            {
                var weatherType = weatherChangeRate.Key;
                var changeRate = weatherChangeRate.Value;

                // rounding is used to counteract some of the floating point arithmetic loss of precision
                this.weatherLevels[weatherType] = Math.Round(this.weatherLevels[weatherType] + changeRate, 4);
                if (this.weatherLevels[weatherType] >= 1.0 || this.weatherLevels[weatherType] <= 0.0)
                {
                    ratesToInvert[weatherType] = true;
                }
            }

            foreach (var rateToInvert in ratesToInvert.Where(rateToInvert => rateToInvert.Value))
            {
                this.weatherChangeRates[rateToInvert.Key] = -this.weatherChangeRates[rateToInvert.Key];
            }
        }
    }
}
