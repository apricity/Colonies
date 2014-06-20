namespace Wacton.Colonies.DataTypes.Interfaces
{
    public interface IMeasurement<T> where T : IMeasure
    {
        T Measure { get; }

        double Level { get; }
    }
}