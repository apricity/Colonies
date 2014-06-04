namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    public interface IMeasurementData
    {
        IEnumerable<IMeasurement> Measurements { get; }

        double GetLevel(IMeasure measure);
    }
}
