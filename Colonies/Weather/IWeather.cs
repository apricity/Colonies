namespace Wacton.Colonies.Weather
{
    public interface IWeather
    {
        double GetLevel(WeatherType weatherType);

        void Advance();
    }
}
