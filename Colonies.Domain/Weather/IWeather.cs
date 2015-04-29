namespace Wacton.Colonies.Domain.Weather
{
    public interface IWeather
    {
        double GetLevel(WeatherType weatherType);

        void Advance();
    }
}
