namespace Wacton.Colonies.Domain.Settings
{
    using System.Collections.Generic;

    using Wacton.Colonies.Domain.Measures;

    public interface IEcosystemSettings
    {
        Dictionary<IMeasure, double> IncreasingRates { get; }
        Dictionary<IMeasure, double> DecreasingRates { get; }
        Dictionary<EnvironmentMeasure, HazardRate> HazardRates { get; }
    }
}