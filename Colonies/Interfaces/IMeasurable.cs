namespace Wacton.Colonies.Interfaces
{
    using Wacton.Colonies.Ancillary;

    public interface IMeasurable
    {
        IMeasurement Measurement { get; }

        double GetLevel(Measure measure);
    }
}