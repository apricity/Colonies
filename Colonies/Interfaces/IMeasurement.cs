namespace Wacton.Colonies.Interfaces
{
    using System.Collections.Generic;

    public interface IMeasurement
    {
        IEnumerable<ICondition> Conditions { get; }

        double GetLevel(IMeasure measure);
    }
}
