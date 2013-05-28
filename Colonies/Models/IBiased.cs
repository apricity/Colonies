namespace Colonies.Models
{
    using System.Collections.Generic;

    public interface IBiased
    {
        Dictionary<Measure, double> GetMeasureBiases();
    }
}
