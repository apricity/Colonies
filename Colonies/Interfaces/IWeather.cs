namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;

    public interface IWeather
    {
        double GetWeatherLevel(WeatherType weatherType);

        void Progress();
    }
}
