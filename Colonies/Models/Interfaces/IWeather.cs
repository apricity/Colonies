namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes.Enums;

    public interface IWeather
    {
        double GetLevel(WeatherType weatherType);

        void Advance();
    }
}
