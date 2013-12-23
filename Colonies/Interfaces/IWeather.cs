namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.Ancillary;

    public interface IWeather
    {
        IEnumerable<WeatherType> WeatherTypes { get; }

        Measure GetWeatherHazard(WeatherType weatherType);

        double GetWeatherLevel(WeatherType weatherType);

        void Progress();
    }
}
