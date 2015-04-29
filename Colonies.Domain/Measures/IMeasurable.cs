namespace Wacton.Colonies.Domain.Measures
{
    public interface IMeasurable<T> where T : IMeasure
    {
        IMeasurementData<T> MeasurementData { get; }

        double GetLevel(T measure);
    }
}