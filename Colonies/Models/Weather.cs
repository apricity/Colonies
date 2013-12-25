namespace Wacton.Colonies.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Wacton.Colonies.Ancillary;
    using Wacton.Colonies.Interfaces;

    public class Weather : IWeather
    {
        // TODO: better naming?
        private Dictionary<WeatherType, Measure> WeatherHazards { get; set; } 
        private Dictionary<WeatherType, double> WeatherLevels { get; set; }
        private Dictionary<WeatherType, double> WeatherChangeRates { get; set; }

        public IEnumerable<WeatherType> WeatherTypes
        {
            get
            {
                return this.WeatherHazards.Keys;
            }
        }

        public Weather()
        {
            this.WeatherHazards = new Dictionary<WeatherType, Measure>
                                      {
                                          { WeatherType.Damp, Measure.Damp },
                                          { WeatherType.Heat, Measure.Heat }
                                      };

            this.WeatherLevels = new Dictionary<WeatherType, double>();
            foreach (var weatherType in this.WeatherTypes)
            {
                this.WeatherLevels.Add(weatherType, 0);
            }

            this.WeatherChangeRates = new Dictionary<WeatherType, double>
                                          {
                                              { WeatherType.Damp, 1 / (double)200 },
                                              { WeatherType.Heat, 1 / (double)2000 }
                                          };
        }

        public Measure GetWeatherHazard(WeatherType weatherType)
        {
            return this.WeatherHazards[weatherType];
        }

        public double GetWeatherLevel(WeatherType weatherType)
        {
            return this.WeatherLevels[weatherType];
        }

        public void Progress()
        {
            var ratesToInvert = this.WeatherChangeRates.ToDictionary(
                weatherChangeRate => weatherChangeRate.Key, weatherChangeRate => false);

            foreach (var weatherChangeRate in this.WeatherChangeRates)
            {
                var weatherType = weatherChangeRate.Key;
                var changeRate = weatherChangeRate.Value;

                // rounding is used to counteract some of the floating point arithmetic loss of precision
                this.WeatherLevels[weatherType] = Math.Round(this.WeatherLevels[weatherType] + changeRate, 4);
                if (this.WeatherLevels[weatherType] >= 1.0 || this.WeatherLevels[weatherType] <= 0.0)
                {
                    ratesToInvert[weatherType] = true;
                }
            }

            foreach (var rateToInvert in ratesToInvert.Where(rateToInvert => rateToInvert.Value))
            {
                this.WeatherChangeRates[rateToInvert.Key] = -this.WeatherChangeRates[rateToInvert.Key];
            }
        }
    }
}
