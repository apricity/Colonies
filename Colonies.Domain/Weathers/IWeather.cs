namespace Wacton.Colonies.Domain.Weathers
{
    public interface IWeather
    {
        double GetLevel(WeatherType weatherType);

        void Advance();
    }
}
