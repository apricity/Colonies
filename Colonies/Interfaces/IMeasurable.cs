namespace Wacton.Colonies.Interfaces
{
    public interface IMeasurable<T> where T : IMeasure
    {
        IMeasurement Measurement { get; }

        double GetLevel(T measure);
    }
}