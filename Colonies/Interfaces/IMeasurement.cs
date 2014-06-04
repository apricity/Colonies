namespace Wacton.Colonies.Interfaces
{
    public interface IMeasurement
    {
        IMeasure Measure { get; }

        double Level { get; }
    }
}