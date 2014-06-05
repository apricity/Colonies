namespace Wacton.Colonies.DataTypes.Interfaces
{
    public interface IMeasurement
    {
        IMeasure Measure { get; }

        double Level { get; }
    }
}