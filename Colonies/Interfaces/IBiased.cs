namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.Ancillary;

    public interface IBiased
    {
        Dictionary<Measure, double> MeasureBiases { get; }

        void SetMeasureBias(Measure measure, double bias);
    }
}
