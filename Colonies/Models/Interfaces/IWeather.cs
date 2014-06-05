namespace Wacton.Colonies.Models.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.DataTypes.Enums;

    public interface IWeather
    {
        IEnumerable<WeatherType> WeatherTypes { get; }

        EnvironmentMeasure GetWeatherHazard(WeatherType weatherType);

        double GetWeatherLevel(WeatherType weatherType);

        void Progress();
    }
}
