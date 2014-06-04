namespace Wacton.Colonies.Interfaces
{
    public interface IMeasurable<T> where T : IMeasure
    {
        IMeasurementData MeasurementData { get; }

        double GetLevel(T measure);
    }
}