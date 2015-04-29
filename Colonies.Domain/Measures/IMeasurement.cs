namespace Wacton.Colonies.Domain.Measures
{
    public interface IMeasurement<T> where T : IMeasure
    {
        T Measure { get; }

        double Level { get; }
    }
}