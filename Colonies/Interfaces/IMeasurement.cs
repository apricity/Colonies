namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    using Wacton.Colonies.Ancillary;

    public interface IMeasurement
    {
        IEnumerable<ICondition> Conditions { get; }

        double GetLevel(Measure measure);
    }
}
