using System.Collections.Generic;

namespace Colonies.Models
{
    public interface IBiased
    {
        Dictionary<Measure, double> GetMeasureBiases();
    }
}
