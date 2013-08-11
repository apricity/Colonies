namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    public interface IBiased
    {
        Dictionary<Measure, double> MeasureBiases { get; }
    }
}
