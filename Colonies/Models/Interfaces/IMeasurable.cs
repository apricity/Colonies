namespace Wacton.Colonies.Models.Interfaces
{
    using Wacton.Colonies.DataTypes.Interfaces;

    public interface IMeasurable<T> where T : IMeasure
    {
        IMeasurementData MeasurementData { get; }

        double GetLevel(T measure);
    }
}